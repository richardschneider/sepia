using Sepia.Calendaring.Serialization;
using System;
using System.Device.Location;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Xml;

// TODO: VAlarm.Local - The local time zone
// TODO: Future: use a Custom TimeZoneInfo.

namespace Sepia.Calendaring
{
    /// <summary>
    ///   A reminder or alarm for a <see cref="VEvent"/> or <see cref="VTodo"/>.
    /// </summary>
    /// <remarks>
    ///   A <b>VAlarm</b> is configured to either trigger on an <see cref="TriggerOn">absolute date/time</see> or
    ///   some <see cref="TriggerDuration">duration</see> that is <see cref="TriggerEdge">relative</see> to the start
    ///   or end of the <see cref="VEvent"/> or <see cref="VTodo"/>.
    /// </remarks>
    public class VAlarm : ICalenderComponent
    {
        DateTime? triggerOn;

        /// <summary>
        ///   Creates a new instance of the <see cref="VAlarm"/> class with the default
        ///   values.
        /// </summary>
        public VAlarm()
        {
            Attendees = new List<Attendee>(0);
            TriggerEdge = TriggerEdge.Start;
        }

        /// <summary>
        ///   The type of action to take when the alarm is triggered.
        /// </summary>
        public AlarmAction Action { get; set; }

        /// <summary>
        ///   The participants who receive the email when the alarm is triggered.
        /// </summary>
        /// <remarks>
        ///   <b>Attendees</b> is only used when <see cref="Action"/> is <see cref="AlarmAction.Email"/>.
        /// </remarks>
        public List<Attendee> Attendees { get; set; }

        /// <summary>
        ///   The length of time to display or play the alarm.
        /// </summary>
        /// <value>
        ///   The default is <see cref="TimeSpan.Zero"/> and indicates no duration.
        /// </value>
        /// <remarks>
        ///   <b>Duration</b> is ignored when the <see cref="Action"/> is <see cref="AlarmAction.Email"/>.
        /// </remarks>
        /// <seealso cref="Repeat"/>
        public TimeSpan Duration { get; set; }

        /// <summary>
        ///   The number of additional times to repeat an alarm.
        /// </summary>
        /// <value>
        ///   The default is zero and indicates only once.
        /// </value>
        /// <remarks>
        ///   When non-zero the <see cref="Duration"/> property must be specified.
        /// </remarks>
        public int Repeat { get; set; }

        /// <summary>
        ///   The <see cref="Text"/> to display or send when the alarm is triggered.
        /// </summary>
        /// <remarks>
        ///   <b>Description</b> is ignored when the <see cref="Action"/> is <see cref="AlarmAction.Audio"/>.
        /// </remarks>
        public Text Description { get; set; }

        /// <summary>
        ///   The subject for an email.
        /// </summary>
        /// <remarks>
        ///   <b>Summary</b> is only used when <see cref="Action"/> is <see cref="AlarmAction.Email"/>.
        /// </remarks>
        public Text Summary { get; set; }

        /// <summary>
        ///   A document (resource) associated with the calendar component.
        /// </summary>
        public CalendarAttachment Attachment { get; set; }

        /// <summary>
        ///   The UTC-relative absolute time that triggers the alarm.
        /// </summary>
        /// <value>
        ///   Must have <see cref="DateTime.Kind"/> equal to <see cref="DateTimeKind.Utc"/>.
        /// </value>
        /// <exception cref="ArgumentException">
        ///   When value's <see cref="DateTime.Kind"/> is not equal to <see cref="DateTimeKind.Utc"/>
        /// </exception>
        /// <remarks>
        ///   <see cref="TriggerOn"/> and <see cref="TriggerDuration"/> are mutually exclusive.
        /// </remarks>
        public DateTime? TriggerOn
        {
            get { return triggerOn; }
            set
            {
                Guard.When(value.HasValue, value.HasValue && value.Value.Kind == DateTimeKind.Utc, "TriggerOn", "Must be a UTC relative date time.");
 
                triggerOn = value;
            }
        }

        /// <summary>
        ///  TODO
        /// </summary>
        public TimeSpan? TriggerDuration { get; set; }

        /// <summary>
        ///   Identifies the base time that is used to determine when a <see cref="VAlarm"/> is triggered.  The actual trigger time is the
        ///   base time plus the <see cref="VAlarm.TriggerDuration"/>.
        /// </summary>
        /// <value>
        ///   The default value is <see cref="Sepia.Calendaring.TriggerEdge.Start"/>.
        /// </value>
        public TriggerEdge TriggerEdge { get; set; }
   
        /// <inheritdoc/>
        public void ReadIcs(IcsReader reader)
        {
            ContentLine content;
            while (null != (content = reader.ReadContentLine()))
            {
                switch (content.Name.ToLowerInvariant())
                {
                    case "end":
                        if (!content.Value.Equals(Component.Names.Alarm, StringComparison.InvariantCultureIgnoreCase))
                            throw new CalendarException(String.Format("Expected 'END:{0}' not '{1}'.", Component.Names.TimeZone, content));
                        return;
                    case "attendee": Attendees.Add(new Attendee(content)); break;
                    case "action": Action = content.ToTag<AlarmAction>(); break;
                    case "trigger":
                    {
                        switch ((content.Parameters["value"] ?? "duration").ToLowerInvariant())
                        {
                            case "date-time":
                                TriggerOn = content.ToDate().Value;
                                break;
                            case "duration":
                                TriggerDuration = content.ToTimeSpan();
                                break;
                        }
                        switch ((content.Parameters["related"] ?? "start").ToLowerInvariant())
                        {
                            case "start":
                                TriggerEdge = TriggerEdge.Start;
                                break;
                            case "end":
                                TriggerEdge = TriggerEdge.End;
                                break;
                        }
                        break;
                    }
                    case "duration": Duration = content.ToTimeSpan(); break;
                    case "attach": Attachment = new CalendarAttachment(content); break;
                    case "description": Description = content.ToText(); break;
                    case "summary": Summary = content.ToText(); break;
                    case "repeat": Repeat = content.ToInt32(); break;

                }
            }

            throw new CalendarException("Unexpected end of file.");
        }

        /// <inheritdoc/>
        public void WriteIcs(IcsWriter ics)
        {
            Guard.IsNotNull(ics, "ics");

            ics.WriteBeginComponent(Component.Names.Alarm);
            ics.WriteContent(Attendees);
            ics.WriteContent("action", Action.Name);
            var trigger = new ContentLine { Name = "trigger" };
            if (TriggerOn.HasValue)
            {
                trigger.Parameters["value"] = "DATE-TIME";
                trigger.Value = IcsWriter.IsoFormat(TriggerOn.Value);
            }
            else if (TriggerDuration.HasValue)
            {
                if (TriggerEdge == TriggerEdge.End)
                    trigger.Parameters["related"] = "END";
                trigger.Value = XmlConvert.ToString(TriggerDuration.Value);
            }
            ics.Write(trigger);
            ics.WriteContent("description", Description);
            ics.WriteContent("summary", Summary);
            ics.WriteContent("duration", Duration);
            ics.WriteContent(Attachment);
            if (Repeat > 0)
                ics.WriteContent("repeat", Repeat);
            ics.WriteEndComponent();
        }
    }
}
