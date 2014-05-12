using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// TODO: Is a FuzzyIsIn sensible.

namespace Sepia
{
    /// <summary>
    ///   Takes clock skew into consideration for <see cref="DateTimeOffset"/>.
    /// </summary>
    /// <remarks>
    ///   Matt Johnson has a good discussion of <see href="http://stackoverflow.com/questions/4331189/datetime-vs-datetimeoffset">calendar vs. instantaneous time</see>. 
    ///   Jon Skeet's <see href="https://code.google.com/p/noda-time/">Noda Time</see> is also a good source for time.
    /// <para> 
    ///   Sepia uses instantaneous time as represented by <see cref="DateTimeOffset"/>. DateTimeOffset supports UTC which is instantaneous and 
    ///   also records the end user perception of time.  Sepia simply refers to it as time; not as date-time because a date implies a calendar system.
    /// </para>
    /// <para>   
    ///   Comparing time is problematic because the clock sources (different nodes on a network) will vary slightly and we cannot guarantee that two times 
    ///   come from the same clock source.  Clock drift is the difference between two clocks and is unavoidable (10 minutes is common in the internet). We need
    ///   to take into consideration the clock drift when comparing times.
    /// </para>
    /// <para>   
    ///   The DateTimeOffset is extended to support drifted comparison (<see cref="O:Sepia.TimeExtensions.FuzzyEquals"/>
    ///   and <see cref="O:Sepia.TimeExtensions.FuzzyCompare"/>).  These methods take an optional acceptable clock 
    ///   drift into consideration. Equality is defined as <b>|T0 - T1| &lt;= Drift</b>.
    /// </para>
    /// <para>   
    ///   Determining if a time is within time period (start and end) is cause for confusion.  Are the time period components inclusive, exclusive or
    ///   some other combination.  Most people perceive that a period starts on a specific time and is finished when the end time is reached.  This implies 
    ///   that start time is inclusive while end time is exclusive.  The <see cref="O:Sepia.TimeExtensions.IsIn"/> extension supplies this behaviour. 
    ///   Normally a "never ends" would be DateTimeOffset.MaxValue, however some platforms may have different max values so a nullable time is used to indicate "never ends".
    /// </para>
    /// <para>   
    ///   Use the "o" format specifier when converting a time to a string, unless presenting the time to an end user.
    /// </para>
    /// </remarks>
    /// <seealso cref="TimeRange"/>
    public static class TimeExtensions
    {
        /// <summary>
        ///   The default acceptable drift (difference) between two clocks.
        /// </summary>
        /// <value>
        ///   The default is 10 minutes for internet clock skew.
        /// </value>
        public static TimeSpan DefaultDrift = TimeSpan.FromMinutes(10);

        /// <summary>
        ///   Determines whether the current <see cref="DateTimeOffset"/> is considered the same point in time as the
        ///   specified <see cref="DateTimeOffset"/> using the <see cref="DefaultDrift"/> of clocks.
        /// </summary>
        /// <param name="a">The first <see cref="DateTimeOffset">time</see> to compare.</param>
        /// <param name="b">The second <see cref="DateTimeOffset">time</see> to compare.</param>
        /// <returns>
        ///   <b>true</b> if both times are consider the same; otherwise, <b>false</b>.
        /// </returns>
        /// <remarks>
        ///   Equality is defined as both times are within the <see cref="DefaultDrift"/> (|a - b| &lt;= <see cref="DefaultDrift"/>).
        /// </remarks>
        public static bool FuzzyEquals(this DateTimeOffset a, DateTimeOffset b)
        {
            return a.FuzzyCompare(b) == 0;
        }

        /// <summary>
        ///   Determines whether the current <see cref="DateTimeOffset"/> is considered the same point in time as the
        ///   specified <see cref="DateTimeOffset"/> using the specified clock drift.
        /// </summary>
        /// <param name="a">The first <see cref="DateTimeOffset">time</see> to compare.</param>
        /// <param name="b">The second <see cref="DateTimeOffset">time</see> to compare.</param>
        /// <param name="drift">The acceptable <see cref="TimeSpan">difference</see> between clocks.</param>
        /// <returns>
        ///   <b>true</b> if both times are consider the same; otherwise, <b>false</b>.
        /// </returns>
        /// <remarks>
        ///   Equality is defined as both times are within the <see cref="DefaultDrift"/> (|a - b| &lt;= <see cref="DefaultDrift"/>).
        /// </remarks>
        public static bool FuzzyEquals(this DateTimeOffset a, DateTimeOffset b, TimeSpan drift)
        {
            return a.FuzzyCompare(b, drift) == 0;
        }

        /// <summary>
        ///   Compares the two times and returns an integer that indicates whether the first time
        ///   precedes, equals or follows the second time using the <see cref="DefaultDrift"/>.
        /// </summary>
        /// <param name="a">The first <see cref="DateTimeOffset">time</see> to compare.</param>
        /// <param name="b">The second <see cref="DateTimeOffset">time</see> to compare.</param>
        /// <returns>TODO</returns>
        public static int FuzzyCompare(this DateTimeOffset a, DateTimeOffset b)
        {
            return Math.Abs(a.Ticks - b.Ticks) <= DefaultDrift.Ticks
                ? 0
                : a.CompareTo(b);
        }

        /// <summary>
        ///   Compares the two times and returns an integer that indicates whether the first time
        ///   precedes, equals or follows the second time using the specified clock drift.
        /// </summary>
        /// <param name="a">The first <see cref="DateTimeOffset">time</see> to compare.</param>
        /// <param name="b">The second <see cref="DateTimeOffset">time</see> to compare.</param>
        /// <param name="drift">The acceptable <see cref="TimeSpan">difference</see> between clocks.</param>
        /// <returns>TODO</returns>
        public static int FuzzyCompare(this DateTimeOffset a, DateTimeOffset b, TimeSpan drift)
        {
            return Math.Abs(a.Ticks - b.Ticks) <= drift.Ticks
                ? 0
                : a.CompareTo(b); 
        }

        /// <summary>
        ///   Determines if the time is in the specified time period.
        /// </summary>
        /// <param name="now">
        ///   The time to compare.
        /// </param>
        /// <param name="inclusiveStart">The inclusive start time.</param>
        /// <param name="exclusiveEnd">The exclusive end time.</param>
        /// <returns>
        ///   <b>true</b> if <paramref name="now"/> within the specified period; otherwise <b>false</b>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="exclusiveEnd"/> must be greater than <paramref name="inclusiveStart"/>.
        /// </exception>
        /// <example>
        ///   <code title="Credit Card Check" source="SepiaExamples\TimeExample.cs" region="Credit Card Check" language="C#" />
        /// </example>
        public static bool Between(this DateTimeOffset now, DateTimeOffset inclusiveStart, DateTimeOffset exclusiveEnd)
        {
            if (exclusiveEnd <= inclusiveStart)
                throw new ArgumentOutOfRangeException("end", exclusiveEnd.ToString("o"), string.Format("The end must be greater than the start '{0:o}'.", inclusiveStart));

            return inclusiveStart <= now && now < exclusiveEnd;
        }


        /// <summary>
        ///   Determines if the time is in the specified time period.
        /// </summary>
        /// <param name="now">
        ///   The time to compare.
        /// </param>
        /// <param name="inclusiveStart">The inclusive start time.</param>
        /// <param name="exclusiveEnd">The exclusive end time or <b>null</b> for never ending.</param>
        /// <returns>
        ///   <b>true</b> if <paramref name="now"/> within the specified period; otherwise <b>false</b>.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///   <paramref name="exclusiveEnd"/> must be greater than <paramref name="inclusiveStart"/>.
        /// </exception>
        /// <example>
        ///   <code title="Credit Card Check" source="SepiaExamples\TimeExample.cs" region="Credit Card Check" language="C#" />
        /// </example>
        public static bool Between(this DateTimeOffset now, DateTimeOffset inclusiveStart, DateTimeOffset? exclusiveEnd)
        {
            if (exclusiveEnd.HasValue && exclusiveEnd.Value <= inclusiveStart)
                throw new ArgumentOutOfRangeException("exclusivEnd", exclusiveEnd.Value, string.Format("The end must be greater than the start '{0}'.", inclusiveStart));

            return inclusiveStart <= now && (!exclusiveEnd.HasValue || now < exclusiveEnd.Value);
        }

        // TODO: FuzzyIsIn

        /// <summary>
        ///   Generates an ordered sequence of time instants starting at this instant of time and
        ///   incrementing by the specified <see cref="TimeSpan"/>. 
        /// </summary>
        /// <param name="start">
        ///   The <see cref="DateTimeOffset"/> to start the sequence at.
        /// </param>
        /// <param name="step">
        ///   The <see cref="TimeSpan"/> to increment by.
        /// </param>
        /// <returns>
        ///   An ordered sequence of dates.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   <paramref name="step"/> is <see cref="TimeSpan.Zero"/>.
        /// </exception>
        /// <remarks>
        ///   This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required 
        ///   to perform the action. The operation represented by this method is not executed until the object is enumerated either by calling its GetEnumerator method 
        ///   directly or by using <c>foreach</c>.
        /// <para>
        ///   The sequence can, in theory, be infinite because each item is generated as needed.  The length of the sequence can be
        ///   controlled by using a <see cref="System.Linq"/> limiter; such as <see cref="System.Linq.Enumerable.Take{T}"/>.
        /// </para>
        /// <para>
        ///   The sequence is terminated on Overflow/Underflow.
        /// </para>
        /// </remarks>      
        public static IEnumerable<DateTimeOffset> StepBy(this DateTimeOffset start, TimeSpan step)
        {
            Guard.Require(step != TimeSpan.Zero, "step", "Cannot step by a zero.");

            var next = start;
            while (true)
            {
                yield return next;

                // Overflow/underflow ends the sequence.
                checked
                {
                    try
                    {
                        next += step;
                    }
                    catch (OverflowException)
                    {
                        break;
                    }
                }
            }
        }

    }
}
