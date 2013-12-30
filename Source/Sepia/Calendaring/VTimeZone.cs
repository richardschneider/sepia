using Sepia.Calendaring.Serialization;
using System;
using System.Device.Location;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

// TODO: Future: use a Custom TimeZoneInfo.

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Represents an unambiguous set of time measurement rules determined by a governing body for a given
    ///   geographic area.
    /// </summary>
    /// <remarks>
    ///   The <see cref="Id"/> property and at least one <see cref="Adjustments">adjustment</see> is required. 
    ///   <note>
    ///     The <see cref="VCalendar.ForLocalTimeZone"/> method creates a default calendar containing the local time zone information.
    ///   </note>
    /// </remarks>
    /// <seealso cref="SeasonalChange"/>
    /// <seealso cref="StandardChange"/>
    /// <seealso cref="DaylightChange"/>
    public class VTimeZone : IResolvable, ICalenderComponent
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="VTimeZone"/> class with the default
        ///   values.
        /// </summary>
        public VTimeZone()
        {
            Adjustments = new List<SeasonalChange>();
        }

        /// <summary>
        ///   The identifier for the geographical area.
        /// </summary>
        /// <remarks>
        ///   This must be unique within the <see cref="VCalendar"/> and is required.
        /// </remarks>
        public string Id { get; set; }

        /// <summary>
        ///   When the time zone information was last revised.
        /// </summary>
        public Date? ModifiedOn { get; set; }

        /// <inheritdoc/>
        public string Uri { get; set; }

        /// <summary>
        ///   The seasonal adjustments made to a time zone.
        /// </summary>
        /// <remarks>
        ///   At least one <seealso cref="SeasonalChange"/> adjustment is required.
        /// </remarks>
        /// <seealso cref="StandardChange"/>
        /// <seealso cref="DaylightChange"/>
        public List<SeasonalChange> Adjustments { get; set; }

        /// <inheritdoc/>
        public void ReadIcs(IcsReader reader)
        {
            ContentLine content;
            while (null != (content = reader.ReadContentLine()))
            {
                switch (content.Name.ToLowerInvariant())
                {
                    case "begin":
                        SeasonalChange adjustment;
                        switch (content.Value.ToLowerInvariant())
                        {
                            case "standard": adjustment = new StandardChange(); break;
                            case "daylight": adjustment = new DaylightChange(); break;
                            default:
                                throw new CalendarException(string.Format("'{0}' is not a component of a time zone.", content.Value));
                        }
                        adjustment.ReadIcs(reader);
                        Adjustments.Add(adjustment);
                        break;

                    case "end":
                        if (!content.Value.Equals(Component.Names.TimeZone, StringComparison.InvariantCultureIgnoreCase))
                            throw new CalendarException(String.Format("Expected 'END:{0}' not '{1}'.", Component.Names.TimeZone, content));
                        return;

                    case "tzid": Id = content.Value; break;
                    case "last-modified": ModifiedOn = content.ToDate(); break;
                    case "url": Uri = content.Value; break;
                }
            }

            throw new CalendarException("Unexpected end of file.");
        }

        /// <inheritdoc/>
        public void WriteIcs(IcsWriter ics)
        {
            Guard.IsNotNull(ics, "ics");

            ics.WriteBeginComponent(Component.Names.TimeZone);
            ics.WriteContent("last-modified", ModifiedOn);
            ics.WriteContent("tzid", Id);
            ics.WriteContent("url", Uri);
            foreach (var adjustment in Adjustments)
            {
                adjustment.WriteIcs(ics);
            }
            ics.WriteEndComponent();
        }

        /// <summary>
        ///   Creates a <see cref="VTimeZone"/> from a <see cref="TimeZoneInfo"/> object.
        /// </summary>
        /// <param name="tzi">
        ///   The <see cref="TimeZoneInfo"/> to convert.
        /// </param>
        /// <returns>
        ///   A <see cref="VTimeZone"/> representing the <paramref name="tzi"/>.
        /// </returns>
        /// <example>
        ///   The following snippet demonstrates how to create an instance of the <see cref="VTimeZone"/> class for the
        ///   local time zone.
        ///  <code>
        ///   var localTimeZone = VTimeZone.FromTimeZoneInfo(TimeZoneInfo.Local);
        ///  </code>
        /// </example>
        /// <seealso cref="VCalendar.ForLocalTimeZone"/>
        public static VTimeZone FromTimeZoneInfo (TimeZoneInfo tzi)
        {
            Guard.IsNotNull(tzi, "tzi");

            var tz = new VTimeZone { Id = tzi.DisplayName };

            // Simple case of no adjustments.
            if (!tzi.SupportsDaylightSavingTime)
            {
                tz.Adjustments.Add(new StandardChange
                {
                    StartsOn = new DateTime(0, DateTimeKind.Local),
                    Name = new Text(CultureInfo.CurrentCulture.Name, tzi.StandardName),
                    OffsetFrom = tzi.BaseUtcOffset,
                    OffsetTo = tzi.BaseUtcOffset
                });
                return tz;
            }

            // Convert TZI adjustments. Each adjustment is DaylightChange.  
            // TODO: Need to synthesize the StandardChange.
            // TODO: Rewrite when RecurrenceRule is functional.
            foreach (var adjustment in tzi.GetAdjustmentRules())
            {
                var daylight = new DaylightChange
                {
                    Name = new Text(CultureInfo.CurrentCulture.Name, tzi.DaylightName),
                    StartsOn = adjustment.DateStart + adjustment.DaylightTransitionStart.TimeOfDay.TimeOfDay,
                    OffsetFrom = tzi.BaseUtcOffset,
                    OffsetTo = tzi.BaseUtcOffset + adjustment.DaylightDelta,
                };
                if (adjustment.DaylightTransitionStart.IsFixedDateRule)
                {
                    throw new NotImplementedException("TimeZoneInfo with fixed date rule.");
                }
                else
                {
                    var s = new StringBuilder("FREQ=YEARLY;WKST=MO");
                    s.AppendFormat(";BYMONTH={0}", adjustment.DaylightTransitionStart.Month);
                    s.AppendFormat(";Day={0}{1}",
                        adjustment.DaylightTransitionStart.Week == 5 ? -1 : adjustment.DaylightTransitionStart.Week,
                        adjustment.DaylightTransitionStart.DayOfWeek.ToRfc5545WeekDayName());
                    daylight.RecurrenceRule = RecurrenceRule.Parse(s.ToString());
                }
                tz.Adjustments.Add(daylight);
            }
            return tz;
        }
    }
}
