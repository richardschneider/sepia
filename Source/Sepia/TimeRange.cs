using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;
using System.Xml;

namespace Sepia
{
    /// <summary>
    ///   Represents an inclusive start and exclusive end <see cref="DateTimeOffset">time</see>.
    /// </summary>
    /// <remarks>
    ///   Determining if a time is within a time range (start and end) is cause for confusion.  Are the time range components inclusive, exclusive or
    ///   some other combination?  Most people perceive that a period starts on a specific time and is finished when the end time is reached.  This implies 
    ///   that start time is inclusive while end time is exclusive.  
    ///  <para>
    ///   A "never ends" is represented as <see cref="EndsOn"/> with a <see cref="DateTimeOffset.MaxValue"/>.
    ///   <note type="warning">The <see cref="DateTimeOffset.MaxValue">last tick of time</see> cannot be represented in a <see cref="TimeRange"/>.</note>
    ///  </para>
    ///  <para>
    ///   Resource scheduling can be implemented using <see cref="Divide"/> and one of the <see cref="O:Sepia.TimeRange.Subtract">Subtract overloads</see>.
    ///  </para>
    /// </remarks>
    /// <example>
    ///   The following example determines when the resource can be scheduled for a 2 hour appointment.
    /// 
    ///   <code title="Resource Scheduling" source="SepiaExamples\TimeRangeExample.cs" region="Scheduling" language="C#" />
    /// </example>
    /// <seealso cref="TimeExtensions"/>
    public struct TimeRange // TODO:  IComparable, ISerializable, IDeserializationCallback ???, IComparable<TimeRange>, IEquatable<TimeRange>
    {
        static readonly Regex WeekPeriod = new Regex(@"(?<weeks>\d+)W((?<days>\d+)D)?");
        static readonly string[] Iso8061DateTimeForms = new string[]
            {
                   "yyyyMMdd'T'HHmmss",
                   "yyyyMMdd'T'HHmmss'Z'",
                   "yyyyMMdd'T'HHmmsszzz",
                   "yyyyMMdd'T'HHmmss'.'fffffff",
                   "yyyyMMdd'T'HHmmss'.'fffffff'Z'",
                   "yyyyMMdd'T'HHmmss'.'fffffffzzz",
                   "yyyy'-'MM'-'dd'T'HH':'mm':'ss",
                   "yyyy'-'MM'-'dd'T'HH':'mm':'ss'Z'",
                   "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz",
                   "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff",
                   "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffff'Z'",
                   "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffzzz",
            };

        DateTimeOffset startsOn;
        DateTimeOffset endsOn;

        // TODO: ToIso8601UtcFormat
        // TODO: XML serialisation

        /// <summary>
        ///   Creates a new instance of the <see cref="TimeRange"/> with the inclusive start date that never ends.
        /// </summary>
        /// <param name="start">The inclusive start time.</param>
        public TimeRange(DateTimeOffset start)
        {
            startsOn = start;
            endsOn = DateTimeOffset.MaxValue;
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="TimeRange"/> with the inclusive start date 
        ///   and exclusive end date.
        /// </summary>
        /// <param name="start">The inclusive start time.</param>
        /// <param name="end">The exclusive end time.</param>
        /// <exception cref="ArgumentException">When <paramref name="end"/> is not greater than <paramref name="start"/>.</exception>
        public TimeRange(DateTimeOffset start, DateTimeOffset end)
        {
            Guard.Check(end > start, "end", "The end time must be greater than the start time.");

            startsOn = start;
            endsOn = end;
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="TimeRange"/> with the inclusive start date 
        ///   and exclusive end date.
        /// </summary>
        /// <param name="start">The inclusive start time.</param>
        /// <param name="end">The exclusive end time.</param>
        /// <exception cref="ArgumentException">When <paramref name="end"/> is not greater than <paramref name="start"/>.</exception>
        public TimeRange(DateTimeOffset start, DateTimeOffset? end)
            : this(start, end ?? DateTimeOffset.MaxValue)
        {
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="TimeRange"/> with the inclusive start date 
        ///   and a duration.
        /// </summary>
        /// <param name="start">The inclusive start time.</param>
        /// <param name="duration">The time interval to the end time.</param>
        /// <exception cref="ArgumentException">
        ///   When <paramref name="start"/> plus <paramref name="duration"/> is not greater than <paramref name="start"/>.
        /// </exception>
        public TimeRange(DateTimeOffset start, TimeSpan duration)
            : this(start, start + duration)
        {
        }

        /// <summary>
        ///   The inclusive start time.
        /// </summary>
        public DateTimeOffset StartsOn { get { return startsOn;  } }

        /// <summary>
        ///   The exclusive end time.
        /// </summary>
        public DateTimeOffset EndsOn { get { return endsOn;  } }

        /// <summary>
        ///  The difference between the <see cref="EndsOn">ending</see> and <see cref="StartsOn">starting</see> times.
        /// </summary>
        /// <returns>
        ///   A <see cref="TimeSpan"/> that represents the difference between the <see cref="EndsOn"/> and <see cref="StartsOn"/>.
        /// </returns>
        public TimeSpan Duration 
        {
            get { return endsOn - startsOn; }
        }

        /// <summary>
        ///   A human readable representation of the time range.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (endsOn == DateTimeOffset.MaxValue)
                return startsOn.ToString() + " - infinity";
            if (Duration <= TimeSpan.FromDays(32))
                return startsOn.ToString() + " for " + Duration.ToString();
            return startsOn.ToString() + " - " + endsOn.ToString();
        }

        /// <summary>
        ///   Determines if the specified <see cref="DateTimeOffset"/> is within in the time range.
        /// </summary>
        /// <param name="time">The time to check.</param>
        /// <returns>
        ///   <b>true</b>, if <paramref name="time"/> is within the time range; otherwise, <b>false</b>.
        /// </returns>
        /// <example>
        ///   <code title="Credit Card Check" source="SepiaExamples\TimeRangeExample.cs" region="Credit Card Check" language="C#" />
        /// </example>
        public bool Contains(DateTimeOffset time)
        {
            return StartsOn <= time 
                && (time < EndsOn || time == DateTimeOffset.MaxValue || EndsOn == DateTimeOffset.MaxValue);
        }

        /// <summary>
        ///   Determines if the specified <see cref="TimeRange"/> intersects this time range.
        /// </summary>
        /// <param name="that">The time to check for intersection.</param>
        /// <returns>
        ///   <b>true</b>, if <paramref name="that"/> intersects this time range; otherwise, <b>false</b>.
        /// </returns>
        public bool Contains(TimeRange that)
        {
            return this.Contains(that.StartsOn) || this.Contains(that.endsOn);
        }

        /// <summary>
        ///   Generates an ordered sequence of time ranges starting at this <see cref="StartsOn"/>
        ///   incrementing by the specified <see cref="TimeSpan"/> until the <see cref="EndsOn"/> is reached. 
        /// </summary>
        /// <param name="step">
        ///   The <see cref="TimeSpan"/> to increment by.
        /// </param>
        /// <returns>
        ///   An ordered sequence of time range.
        /// </returns>
        /// <exception cref="ArgumentException">
        ///   <paramref name="step"/> is less than or equal <see cref="TimeSpan.Zero"/>.
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
        ///   The sequence is terminated on Overflow.
        /// </para>
        /// <para>
        ///   When <paramref name="step"/> does not divide evenly into the <see cref="Duration"/>, then the "last" time range is not returned.
        ///   In other words, only time ranges with a <see cref="Duration"/> equal to <paramref name="step"/> is returned.
        /// </para>
        /// </remarks>      
        /// <example>
        ///   The following example divides a <see cref="TimeRange"/> into 15 minutes slots.
        /// 
        ///   <code title="Dividing a Time Range" source="SepiaExamples\TimeRangeExample.cs" region="Dividing" language="C#" />
        /// </example>
        public IEnumerable<TimeRange> Divide(TimeSpan step)  // TODO: maybe rename to split
        {
            Guard.Check(step > TimeSpan.Zero, "step", "Cannot step by a zero or negative number.");

            var next = startsOn;
            while (next < endsOn)
            {
                var value = new TimeRange(next, step);
                if (value.Duration != step || value.endsOn > endsOn)
                    break;
                yield return value;

                // Overflow ends the sequence.
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

        /// <summary>
        ///   Generates an ordered sequence of time ranges that excludes the specified <see cref="TimeRange"/>
        ///   from this <see cref="TimeRange"/>.
        /// </summary>
        /// <param name="other">
        ///   The <see cref="TimeRange"/> to exclude.
        /// </param>
        /// <returns>
        ///   An ordered sequence of time range.  The sequence can contain 0, 1 or 2 time ranges.
        /// </returns>
        /// <remarks>
        ///   This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required 
        ///   to perform the action. The operation represented by this method is not executed until the object is enumerated either by calling its GetEnumerator method 
        ///   directly or by using <c>foreach</c>.
        /// </remarks>      
        /// <example>
        ///   The following example determines the free time for a resource..
        /// 
        ///   <code title="Subtracting a Time Range" source="SepiaExamples\TimeRangeExample.cs" region="SubtractCollection" language="C#" />
        /// </example>
        public IEnumerable<TimeRange> Subtract(TimeRange other)  // TODO: maybe rename to Remove
        {
            if (other.StartsOn <= this.StartsOn && other.EndsOn >= this.EndsOn)
                yield break;
            if (other.EndsOn <= this.StartsOn || this.EndsOn <= other.StartsOn)
            {
                yield return this;
                yield break;
            }
            if (other.StartsOn <= this.StartsOn && other.EndsOn < this.EndsOn)
            {
                yield return new TimeRange(other.EndsOn, this.EndsOn);
                yield break;
            }
            if (other.StartsOn < this.EndsOn && this.StartsOn != other.StartsOn)
                yield return new TimeRange(this.StartsOn, other.StartsOn);
            if (other.endsOn < this.EndsOn && other.EndsOn != this.EndsOn)
                yield return new TimeRange(other.EndsOn, this.EndsOn);
        }

        /// <summary>
        ///   Generates an ordered sequence of time ranges that excludes the specified <see cref="TimeRange"/> collection
        ///   from this <see cref="TimeRange"/>.
        /// </summary>
        /// <param name="others">
        ///   The <see cref="TimeRange"/> to exclude.
        /// </param>
        /// <returns>
        ///   An ordered sequence of time ranges.
        /// </returns>
        /// <remarks>
        ///   This method is implemented by using deferred execution. The immediate return value is an object that stores all the information that is required 
        ///   to perform the action. The operation represented by this method is not executed until the object is enumerated either by calling its GetEnumerator method 
        ///   directly or by using <c>foreach</c>.
        /// </remarks>      
        /// <example>
        ///   The following example determines the free time for a resource..
        /// 
        ///   <code title="Subtracting a Time Range" source="SepiaExamples\TimeRangeExample.cs" region="SubtractCollection" language="C#" />
        /// </example>
        public IEnumerable<TimeRange> Subtract(ICollection<TimeRange> others)
        {
            var prev = new List<TimeRange>(new[] { this });
            foreach (var b in others)
            {
                var next = new List<TimeRange>();
                foreach (var a in prev)
                {
                    next.AddRange(a.Subtract(b));
                }
                prev = next;
            }
            return prev;
        }

        /// <summary>
        ///   Converts the ISO 8061 time-interval into a <see cref="TimeRange"/>. 
        /// </summary>
        /// <param name="s">
        ///   A string in the form 'date-time/date-time' or 'date-time/duration'.
        /// </param>
        /// <returns>
        ///   A <see cref="TimeRange"/> the represents the ISO 8061 time-interval.
        /// </returns>
        /// <exception cref="FormatException">
        ///   When <paramref name="s"/> is not an ISO 8061 time-interval.
        /// </exception>
        /// <example>
        ///   Some sample ISO 8061 time-intervals -
        /// <code>
        ///  TimeRange.ParseIso8061("20130813T090000Z/20130813T093000Z")
        ///  TimeRange.ParseIso8061("20130813T090000+1200/20130813T093000+1200")
        ///  TimeRange.ParseIso8061("20130813T090000Z/PT30M")
        ///  TimeRange.ParseIso8061("20130813T090000+1200/PT30M")
        ///  TimeRange.ParseIso8061("2013-08-13T09:00:00Z/PT30M")
        /// </code>
        /// </example>
        public static TimeRange ParseIso8061(string s)
        {
            Guard.IsNotNullOrWhiteSpace(s, "s");

            var parts = s.Split('/');
            if (parts.Length != 2)
                throw new FormatException("Expected a '/' to separate the time-interval components.");

            var styles = DateTimeStyles.AllowWhiteSpaces |
                (parts[0].EndsWith("Z") ? DateTimeStyles.AssumeUniversal : DateTimeStyles.AssumeLocal);
            var start = DateTimeOffset.ParseExact(parts[0], Iso8061DateTimeForms, CultureInfo.InvariantCulture, styles);

            DateTimeOffset end;
            if (parts[1].StartsWith("P"))
            {
                var v = parts[1];
                // XmlConvert does not cope with a leading '-'.
                if (v.StartsWith("+"))
                    v = v.Substring(1);

                // XmlConvert does not cope with weeks 'P1W', but we can convert it to days 'P7D'.
                if (v.Contains("W"))
                {
                    v = WeekPeriod.Replace(v, (match) =>
                    {
                        var days = Int32.Parse(match.Groups["weeks"].Value) * 7;
                        if (match.Groups["days"].Success)
                            days += Int32.Parse(match.Groups["days"].Value);
                        return days.ToString(CultureInfo.InvariantCulture) + "D";
                    });
                }

                end = start + XmlConvert.ToTimeSpan(v);
            }
            else
            {
                styles = DateTimeStyles.AllowWhiteSpaces |
                    (parts[1].EndsWith("Z") ? DateTimeStyles.AssumeUniversal : DateTimeStyles.AssumeLocal);
                end = DateTimeOffset.ParseExact(parts[1], Iso8061DateTimeForms, CultureInfo.InvariantCulture, styles);
            }

            return new TimeRange(start, end);
        }

        /// <summary>
        ///   Determines the upper exclusive bound for a partial date or date/date.
        /// </summary>
        /// <param name="partial">
        ///   A valid ISO 8061 partial date/time, such as "2012-01-14", "20120114T10".
        /// </param>
        /// <returns>
        ///   A <see cref="TimeRange"/> with <see cref="TimeRange.EndsOn"/> being the next step for
        ///   the partial date/time.
        /// </returns>
        /// <remarks>
        ///   The precision of the partial date/time is determined and then the upper bound is calculated.  For example,
        ///   "2012" has upper bound of "2013" and "2012-01-14" has upper bound of "2012-01-15".  Precision is down to
        ///   seconds.  All valid ISO forms (2012-01-14T10 or 20120114T10) are allowed.
        /// </remarks>
        public static TimeRange FromPartial(string partial)
        {
            Guard.IsNotNullOrWhiteSpace(partial, "partial");

            partial = partial.Replace(":", "").Replace("-", "");

            // Remove milliseconds.
            var pi = partial.IndexOf('.');
            if (pi > 0)
            {
                do
                {
                    partial = partial.Remove(pi, 1);
                } while (pi < partial.Length && char.IsDigit(partial[pi]));
            }

            var styles = (partial.EndsWith("Z") ? DateTimeStyles.AssumeUniversal : DateTimeStyles.AssumeLocal);
            DateTimeOffset start;
            if (DateTimeOffset.TryParseExact(partial, "yyyy", CultureInfo.InvariantCulture, styles, out start))
                return new TimeRange(start, start.AddYears(1));
            if (DateTimeOffset.TryParseExact(partial, "yyyyMM", CultureInfo.InvariantCulture, styles, out start))
                return new TimeRange(start, start.AddMonths(1));
            if (DateTimeOffset.TryParseExact(partial, "yyyyMMdd", CultureInfo.InvariantCulture, styles, out start))
                return new TimeRange(start, start.AddDays(1));
            if (DateTimeOffset.TryParseExact(partial, "yyyyMMddTHH", CultureInfo.InvariantCulture, styles, out start))
                return new TimeRange(start, start.AddHours(1));
            if (DateTimeOffset.TryParseExact(partial, "yyyyMMdd'T'HHmm", CultureInfo.InvariantCulture, styles, out start))
                return new TimeRange(start, start.AddMinutes(1));
            if (DateTimeOffset.TryParseExact(partial, "yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture, styles, out start))
                return new TimeRange(start, start.AddSeconds(1));
            if (DateTimeOffset.TryParseExact(partial, "yyyyMMdd'T'HHmmss'Z'", CultureInfo.InvariantCulture, styles, out start))
                return new TimeRange(start, start.AddSeconds(1));
            if (DateTimeOffset.TryParseExact(partial, "yyyyMMdd'T'HHmmsszzz", CultureInfo.InvariantCulture, styles, out start))
                return new TimeRange(start, start.AddSeconds(1));

            throw new FormatException(string.Format("Unknown ISO 8061 date/time '{0}'.", partial));
        }
    }
}
