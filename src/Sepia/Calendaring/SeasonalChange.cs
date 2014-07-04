using Sepia.Calendaring.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///    Represents the seasonal adjustments to a <see cref="VTimeZone"/>.
    /// </summary>
    /// <remarks>
    ///   The <see cref="StartsOn"/>, <see cref="OffsetFrom"/> and <see cref="OffsetTo"/> properties are required.
    /// </remarks>
    public abstract class SeasonalChange : ICalenderComponent
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="SeasonalChange"/> class with the
        ///   default values.
        /// </summary>
        protected SeasonalChange()
        {
            Comment = new MultilingualText();
            RecurrenceDates = new List<Date>(0);
        }

        /// <summary>
        ///   The effective onset date and local time for the time zone sub-component definition.
        /// </summary>
        /// <value>
        ///   The value is required.
        /// </value>
        /// <remarks>
        ///   The <see cref="DateTime.Kind"/> must be <see cref="DateTimeKind.Local"/>.
        /// </remarks>
        public DateTime StartsOn { get; set; }

        /// <summary>
        ///   The UTC offset that is in use when the onset of this time zone observance begins.
        /// </summary>
        /// <value>
        ///   The value is required.
        /// </value>
        /// <remarks>
        ///   <b>OffsetFrom</b> is combined with <see cref="StartsOn"/> to define the effective
        ///   onset for the time zone sub-component definition.
        /// </remarks>
        public TimeSpan OffsetFrom { get; set; }

        /// <summary>
        ///   The UTC offset for the time zone sub-component (<see cref="StandardChange"/> or <see cref="DaylightChange"/>)
        ///   when this observance is in use.
        /// </summary>
        /// <value>
        ///   The value is required.
        /// </value>
        public TimeSpan OffsetTo { get; set; }
        
        /// <summary>
        ///   The onset of this time zone observance by giving the individual onset date and time(s).
        /// </summary>
        /// <remarks>
        ///   <b>RecurrenceDates</b> must be specified as a date with local time value,
        ///   relative to the UTC offset specified in <see cref="OffsetFrom"/>.
        /// </remarks>
        /// <seealso cref="RecurrenceRule"/>
        public List<Date> RecurrenceDates { get; set; }

        /// <summary>
        ///   The  rule for the onset of this time zone observance.
        /// </summary>
        /// <seealso cref="RecurrenceDates"/>
        public RecurrenceRule RecurrenceRule { get; set; }

        /// <summary>
        ///   The customary name for the time zone adjustment.
        /// </summary>
        public Text Name { get; set; }

        /// <summary>
        ///   Non-processing information intended to provide a comment to the calendar user.
        /// </summary>
        public MultilingualText Comment { get; set; }

        /// <inheritdoc />
        public void ReadIcs(IcsReader reader)
        {
            ContentLine content;
            while (null != (content = reader.ReadContentLine()))
            {
                switch (content.Name.ToLowerInvariant())
                {
                    case "end":
                        return;

                    case "dtstart": StartsOn = content.ToLocalDateTime(); break;
                    case "tzoffsetfrom": OffsetFrom = content.ToTimeZoneOffset(); break;
                    case "tzoffsetto": OffsetTo = content.ToTimeZoneOffset(); break;
                    case "tzname": Name = content.ToText(); break;
                    case "comment": Comment.Add(content.ToText()); break;
                    case "rdate": RecurrenceDates.AddRange(content.ToRecurrenceDates()); break;
                    case "rrule": RecurrenceRule = RecurrenceRule.Parse(content.Value); break;
                }
            }

            throw new CalendarException("Unexpected end of file.");
        }

        /// <inheritdoc/>
        public void WriteIcs(IcsWriter ics)
        {
            Guard.IsNotNull(ics, "ics");

            ics.WriteBeginComponent(ComponentName);
            ics.WriteContent("tzname", Name);
            ics.WriteContent("dtstart", StartsOn);
            ics.WriteContentAsUtcOffset("tzoffsetfrom", OffsetFrom);
            ics.WriteContentAsUtcOffset("tzoffsetto", OffsetTo);
            ics.WriteContent("comment", Comment);
            ics.WriteContent("rdate", RecurrenceDates);
            ics.WriteContent(RecurrenceRule);
            ics.WriteEndComponent();
        }

        /// <summary>
        ///   Gets the component name of the seasonal change (Standard or Daylight)
        /// </summary>
        public abstract string ComponentName { get; }

    }

    /// <summary>
    ///    Represents the winter time <see cref="SeasonalChange"/> to a <see cref="VTimeZone"/>.
    /// </summary>
    public class StandardChange : SeasonalChange
    {

        /// <inheritdoc/>
        public override string ComponentName
        {
            get
            {
                return Component.Names.Standard;
            }
        }
    }

    /// <summary>
    ///    Represents the summer time <see cref="SeasonalChange"/> to a <see cref="VTimeZone"/>.
    /// </summary>
    public class DaylightChange : SeasonalChange
    {
        /// <inheritdoc/>
        public override string ComponentName
        {
            get
            {
                return Component.Names.DayLight;
            }
        }
       
    }
}
