using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Represents a date or a date time.  The date time can be relative to the local time, UTC or a time zone.
    /// </summary>
    public struct Date
    {
        DateTime dateTime;
        string timeZone;
        bool isDateOnly;

        /// <summary>
        ///   Creates a new <see cref="Date"/> class.
        /// </summary>
        /// <param name="dateTime"></param>
        /// <param name="timeZone"></param>
        /// <param name="isDateOnly"></param>
        public Date(DateTime dateTime, string timeZone, bool isDateOnly = false)
        {
            Guard.When(isDateOnly, dateTime.TimeOfDay == TimeSpan.Zero, "dateTime", "The time component must be zero.");
            Guard.When(isDateOnly, dateTime.Kind == DateTimeKind.Unspecified, "dateTime", "The DateTime.Kind must be unspecified.");
            Guard.When(!isDateOnly, dateTime.Kind != DateTimeKind.Unspecified, "dateTime", "The DateTime.Kind must not be unspecified.");
            Guard.When(dateTime.Kind == DateTimeKind.Utc, timeZone == null, "timeZone", "The time zone must be null for a UTC value.");

            this.dateTime = dateTime;
            this.timeZone = timeZone;
            this.isDateOnly = isDateOnly;
        }

        /// <summary>
        ///   TODO
        /// </summary>
        public DateTime Value { get { return dateTime; } }

        /// <summary>
        ///   TODO
        /// </summary>
        public string TimeZone { get { return timeZone; } }

        /// <summary>
        ///   TODO
        /// </summary>
        public bool IsDateOnly { get { return isDateOnly; } }

        /// <summary>
        ///   A human readable representation of the date/time.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (isDateOnly)
                return dateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture);
            if (dateTime.Kind == DateTimeKind.Utc)
                return dateTime.ToString("yyyyMMdd'T'HHmmssZ", CultureInfo.InvariantCulture);
            
            return string.Format("{0:yyyyMMdd'T'HHmmss} ({1})", dateTime, timeZone ?? "local");
        }

        /// <summary>
        ///   TODO
        /// </summary>
        /// <param name="s"></param>
        /// <param name="timezone"></param>
        /// <returns></returns>
        public static Date Parse(string s, string timezone = null)
        {
            Guard.IsNotNullOrWhiteSpace(s, "s");

            if (s.Length == 8)
                return new Date(DateTime.ParseExact(s, "yyyyMMdd", CultureInfo.InvariantCulture), timezone, true);

            DateTime v;
            DateTimeKind kind;

            if (s.EndsWith("Z"))
            {
                v = DateTime.ParseExact(s.Substring(0, s.Length-1), "yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture);
                kind = DateTimeKind.Utc;
            }
            else
            {
                v = DateTime.ParseExact(s, "yyyyMMdd'T'HHmmss", CultureInfo.InvariantCulture);
                kind = DateTimeKind.Local;
            }
            
            return new Date(new DateTime(v.Ticks, kind), timezone);
        }
    }
}
