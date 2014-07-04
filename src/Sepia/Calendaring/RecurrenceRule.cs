using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// TODO: what about UNTIL parameter in content line?

namespace Sepia.Calendaring
{
    using Sepia.Calendaring.Serialization;

    internal static class Rfc544Extensions
    {
        internal static string ToRfc5545WeekDayName(this DayOfWeek dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case DayOfWeek.Sunday: return "SU";
                case DayOfWeek.Monday: return "MO";
                case DayOfWeek.Tuesday: return "TU";
                case DayOfWeek.Wednesday: return "WE";
                case DayOfWeek.Thursday: return "TH";
                case DayOfWeek.Friday: return "FR";
                case DayOfWeek.Saturday: return "SA";
                default:
                    throw new InvalidOperationException("Not a valid DayOfWeek");
            }
        }
    }

    /// <summary>
    ///   TODO
    /// </summary>
    public class RecurrenceRule : IcsSerializable
    {
        /// <summary>
        ///   The abbreviated day names specified by RFC 5545.
        /// </summary>
        /// <remarks>
        ///   These names (all upper case) are in the same order as an ISO day of week. However, <b>IsoDayOfWeek</b> is 1-relative
        ///   and arrays are always 0-relative!
        /// </remarks>
        static internal readonly string[] Rfc5545WeekDayNames = { "MO", "TU", "WE", "TH", "FR", "SA", "SU" };

        string rule;

        /// <summary>
        ///   Create a new <see cref="RecurrenceRule"/> from the specified RFC 5544 "RRULE" <see cref="string"/>.
        /// </summary>
        /// <param name="s">
        ///   A RFC 5545 "RRULE" <see cref="string"/>.
        /// </param>
        /// <returns>
        ///   A <see cref="RecurrenceRule"/>.
        /// </returns>
        /// <example>
        ///   <code>
        ///   var rrule = "RRULE:FREQ=MONTHLY;BYDAY=FR;BYMONTHDAY=13";
        ///   var start = ...;
        ///   foreach (var instance in ReccurenceRule.Parse(rrule).Enumerate(start))
        ///   {
        ///       Console.WriteLine(instance);
        ///   }
        ///   </code>
        /// </example>
        public static RecurrenceRule Parse(string s)
        {
            // TODO: what about UNTIL parameter in content line?
            // TODO
            var rule = new RecurrenceRule();
            rule.rule = s;
            return rule;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return rule;
        }

        /// <inheritdoc />
        public void ReadIcs(IcsReader reader) // TODO
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void WriteIcs(IcsWriter writer) // TODO
        {
            writer.WriteContent("rrule", rule);
        }
    }
}
