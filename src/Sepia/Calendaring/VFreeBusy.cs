using Sepia.Calendaring.Serialization;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///    Represents free/busy time(s) for a calendar user.
    /// </summary>
    /// <remarks>
    ///   The <b>VFreeBusy</b> calendar component can be used to
    /// <list type="bullet">
    ///   <item>request free/busy time information,</item>
    ///   <item>reply to a free/busy time request, or</item>
    ///   <item>publish busy time information</item>
    /// </list>
    /// <para>
    ///  When used to request free/busy time information, the <see cref="Attendee"/>
    ///  property specifies the calendar users whose free/busy time is
    ///  being requested; the <see cref="Organizer"/>" property specifies the calendar
    ///  user who is requesting the free/busy time; the <see cref="StartsOn"/> and
    ///  <see cref="EndsOn"/> properties specify the window of time for which the free/ busy 
    ///  time is being requested; the <see cref="Id"/> and "<see cref="CreatedOnByAgent"/> properties
    ///  are specified to assist in proper sequencing of multiple free/busy
    ///  time requests.
    /// </para>
    /// <para>
    ///  When used to reply to a request for free/busy time, the <see cref="Attendee"/>
    ///  property specifies the calendar user responding to the free/busy
    ///  time request; the <see cref="Organizer"/> property specifies the calendar user
    ///  that originally requested the free/busy time; the <see cref="FreeBusyTimes"/>
    ///  property specifies the free/busy time information (if it exists);
    ///  and the <see cref="Id"/> and "<see cref="CreatedOnByAgent"/> properties are specified to assist in
    ///  proper sequencing of multiple free/busy time replies.
    /// </para>
    /// <para>
    ///  When used to publish busy time, the <see cref="Organizer"/> property specifies
    ///  the calendar user associated with the published busy time; the
    ///  <see cref="StartsOn"/> and <see cref="EndsOn"/> properties specify an inclusive time window
    ///  that surrounds the busy time information; the <see cref="FreeBusyTimes"/> property
    ///  specifies the published busy time information; and the <see cref="CreatedOnByAgent"/>
    ///  property specifies the date and time that iCalendar object was
    ///  created.
    /// </para>
    /// </remarks>
    public class VFreeBusy : IResolvable, ICalenderComponent
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="VFreeBusy"/> class with the default
        ///   values.
        /// </summary>
        public VFreeBusy()
        {
            Id = Guid.NewGuid().ToString();
            RequestStatuses = new List<RequestStatus>(0);
            Attendees = new List<Attendee>(0);
            FreeBusyTimes = new List<TimeSlot>(0);
        }

        /// <summary>
        ///   The globally unique identifier.
        /// </summary>
        /// <value>
        ///   The default value is <see cref="Guid.NewGuid"/>.
        /// </value>
        public string Id { get; set; }

        /// <summary>
        ///   When the component was created by the calendar user agent.
        /// </summary>
        public Date CreatedOnByAgent { get; set; }

        /// <summary>
        ///   Contact information (textual only).
        /// </summary>
        public Contact Contact { get; set; }

        /// <summary>
        ///   When the free/busy time begins (inclusive).
        /// </summary>
        /// <remarks>
        ///   Specifies the inclusive start of the event.  For recurring events, it also specifies the
        ///   very first instance in the recurrence set.
        /// </remarks>
        public Date? StartsOn { get; set; }

        /// <summary>
        ///   When the free/busy stops (exclusive).
        /// </summary>
        /// <remarks>
        ///   If specified it must be greater than or equal to <see cref="StartsOn"/>.
        /// </remarks>
        public Date? EndsOn { get; set; }

        /// <summary>
        ///   One or more free or busy time intervals.
        /// </summary>
        public List<TimeSlot> FreeBusyTimes { get; set; }

        /// <summary>
        ///   A calendar user.
        /// </summary>
        /// <remarks>
        ///   For a request or reply, this is the calendar user that initiated the request.  For a publish, this is
        ///   the calendar user associated with the published busy time.
        /// </remarks>
        public MailAddress Organizer { get; set; }

        /// <inheritdoc/>
        public string Uri { get; set; }

        /// <summary>
        ///   The calendar users whose free/busy time is being requested or replied to.
        /// </summary>
        public List<Attendee> Attendees { get; set; }

        /// <summary>
        ///   Non-processing information intended to provide a comment to the calendar user.
        /// </summary>
        public MultilingualText Comment { get; set; }

        /// <summary>
        ///   The status(es) of a scheduling request.
        /// </summary>
        public List<RequestStatus> RequestStatuses { get; set; }

        /// <inheritdoc/>
        public void ReadIcs(IcsReader reader)
        {
            ContentLine content;
            while (null != (content = reader.ReadContentLine()))
            {
                switch (content.Name.ToLowerInvariant())
                {
                    case "end":
                        if (!content.Value.Equals(Component.Names.FreeBusy, StringComparison.InvariantCultureIgnoreCase))
                            throw new CalendarException(String.Format("Expected 'END:{0}' not '{1}'.", Component.Names.FreeBusy, content));
                        return;
                    case "dtstamp":
                        CreatedOnByAgent = content.ToDate();
                        break;
                    case "uid":
                        Id = content.Value;
                        break;
                    case "contact":
                        Contact = new Contact(content);
                        break;
                    case "dtstart":
                        StartsOn = content.ToDate();
                        break;
                    case "dtend":
                        EndsOn = content.ToDate();
                        break;
                    case "organizer":
                        Organizer = content.ToMailAddress();
                        break;
                    case "url":
                        Uri = content.Value;
                        break;
                    case "attendee":
                        Attendees.Add(new Attendee(content));
                        break;
                    case "comment":
                        Comment.Add(content.ToText());
                        break;
                    case "request-status":
                        RequestStatuses.Add(new RequestStatus(content));
                        break;
                    case "freebusy":
                        FreeBusyTimes.Add(new TimeSlot(content));
                        break;
                }
            }

            throw new CalendarException("Unexpected end of file.");
        }

        /// <inheritdoc/>
        public void WriteIcs(IcsWriter ics)
        {
            Guard.IsNotNull(ics, "ics");

            ics.WriteBeginComponent(Component.Names.FreeBusy);
            ics.WriteContent("organizer", Organizer);
            ics.WriteContent("comment", Comment);
            ics.WriteContent("dtstamp", CreatedOnByAgent);
            ics.WriteContent("dtend", EndsOn);
            ics.WriteContent("uid", Id);
            ics.WriteContent("dtstart", StartsOn);
            ics.WriteContent("url", Uri);
            ics.WriteContent(Attendees);
            ics.WriteContent(RequestStatuses);
            ics.WriteContent(Contact);
            ics.WriteContent(FreeBusyTimes);
            ics.WriteEndComponent();
        }
    }
}
