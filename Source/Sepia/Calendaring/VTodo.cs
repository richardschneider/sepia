using Sepia.Calendaring.Serialization;
using System;
using System.Device.Location;
using System.Net.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///    Represents an to-do item that must performed by a given time in a <see cref="VCalendar"/>.
    /// </summary>
    /// <remarks>
    ///    A <see cref="VTodo"/> calendar component without the <see cref="StartsOn"/> and <see cref="DueOn"/>" (or
    ///    <see cref="Duration"/>) properties specifies a to-do that will be associated
    ///    with each successive calendar date, until it is completed.
    /// </remarks>
    public class VTodo : IResolvable, ICalenderComponent
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="VTodo"/> class with the default
        ///   values.
        /// </summary>
        public VTodo()
        {
            Id = Guid.NewGuid().ToString();
            Classification = AccessClassification.Public;
            RecurrenceDates = new List<Date>(0);
            ExcludedDates = new List<Date>(0);
            Categories = new TagBag();
            RequestStatuses = new List<RequestStatus>(0);
            Attendees = new List<Attendee>();
            Contacts = new List<Contact>(0);
            Resources = new List<Text>(0);
            Attachments = new List<CalendarAttachment>(0);
            Relationships = new List<RelationshipReference>(0);
        }

        /// <summary>
        ///   The globally unique identifier for the event.
        /// </summary>
        /// <value>
        ///   The default value is <see cref="Guid.NewGuid"/>.
        /// </value>
        public string Id { get; set; }

        /// <inheritdoc/>
        public string Uri { get; set; }

        /// <summary>
        ///   The revision sequence number.
        /// </summary>
        /// <remarks>
        ///   When created, the revision is 0.  It is monotonically incremented by the Organizer's
        ///   CUA each time the "Organizer" makes a significant revision.
        /// </remarks>
        public int Revision { get; set; }

        /// <summary>
        ///   When the event was first entered into the calendar store.
        /// </summary>
        public Date? CreatedOn { get; set; }

        /// <summary>
        ///   When the event was last revised in the calendar store.
        /// </summary>
        public Date? ModifiedOn { get; set; }

        /// <summary>
        ///   When the event was created by the calendar user agent.
        /// </summary>
        public Date CreatedOnByAgent { get; set; }

        /// <summary>
        ///   The access classification for the event.
        /// </summary>
        ///   The default value is <see cref="AccessClassification.Public"/>.
        /// <remarks>
        ///   Provides a method of capturing the scope of the access the calendar owner intends for event information.
        ///  <para>
        ///   Applications MUST treat tags values they don't recognize the same way as they would the 
        ///   <see cref="AccessClassification.Private"/> value.
        ///  </para> 
        /// </remarks>
        public AccessClassification Classification { get; set; } 

        /// <summary>
        ///   A short summary or subject.
        /// </summary>
        public Text Summary { get; set; }

        /// <summary>
        ///   A more complete description than that provided by <see cref="Summary"/>.
        /// </summary>
        public Text Description { get; set; }

        /// <summary>
        ///   The coordinating person.
        /// </summary>
        public MailAddress Organizer { get; set; }

        /// <summary>
        ///   The global position.
        /// </summary>
        /// <seealso cref="Location"/>
        public GeoCoordinate GlobalPosition { get; set; }

        /// <summary>
        ///   The intended venue.
        /// </summary>
        /// <seealso cref="GlobalPosition"/>
        public Text Location { get; set; }

        /// <summary>
        ///   Allows the reference to an individual instance within the recurrence set.
        /// </summary>
        /// <remarks>
        ///   <b>RecurrenceId</b> is used in conjunction with the <see cref="Id"/>
        ///   and <see cref="Revision"/> properties to identify a particular instance of a
        ///   recurring event.
        /// </remarks>
        public Date? RecurrenceId { get; set; }

        /// <summary>
        ///   The rule or repeating pattern for a recurring event.
        /// </summary>
        /// <seealso cref="RecurrenceDates"/>
        public RecurrenceRule RecurrenceRule { get; set; }

        /// <summary>
        ///   The aggregate set of repeating occurrences.
        /// </summary>
        /// <remarks>
        ///   When both <see cref="RecurrenceRule"/> and <b>RecurrenceDates</b> appear in a recurring component, the 
        ///   recurrence instances are defined by the union of occurrences defined by both the rule and dates.
        ///   <note type="warning">Support for date periods is not yet implemented.</note>
        /// </remarks>
        /// <seealso cref="RecurrenceRule"/>
        public List<Date> RecurrenceDates { get; set; }

        /// <summary>
        ///   The date time(s) excluded from the resultant set.
        /// </summary>
        public List<Date> ExcludedDates { get; set; }

        /// <summary>
        ///   The relative priority.
        /// </summary>
        ///   The default value is zero (undefined).
        /// <remarks>
        ///   Priority is specified as an integer in the range 0 to 9.  A value of 0 specifies an undefined priority.  
        ///   A value of 1 is the highest priority.  A value of 2 is the second highest priority.  Subsequent numbers 
        ///   specify a decreasing ordinal priority.  A value of 9 is the lowest priority.
        /// </remarks>
        public int Priority { get; set; }

        /// <summary>
        ///   The overall status or confirmation.
        /// </summary>
        public TodoStatus Status { get; set; }

        /// <summary>
        ///   When the to-do begins (inclusive).
        /// </summary>
        /// <remarks>
        ///   Specifies the inclusive start of the to-do.  For recurring to-do, it also specifies the
        ///   very first instance in the recurrence set.
        /// </remarks>
        public Date? StartsOn { get; set; }

        /// <summary>
        ///   When the to-do is due.
        /// </summary>
        /// <remarks>
        ///   Mutually exclusive with <see cref="Duration"/>.
        /// </remarks>
        public Date? DueOn { get; set; }

        /// <summary>
        ///   The length of the event.
        /// </summary>
        /// <remarks>
        ///   Mutually exclusive with <see cref="DueOn"/>.  <see cref="StartsOn"/> is required.
        /// </remarks>
        public TimeSpan? Duration { get; set; }

        /// <summary>
        ///   Any document (resource) associated with the calendar component.
        /// </summary>
        public List<CalendarAttachment> Attachments { get; set; }

        /// <summary>
        ///   Any associated participant.
        /// </summary>
        public List<Attendee> Attendees { get; set; }

        /// <summary>
        ///   The categories or subtypes of the calendar component.
        /// </summary>
        public TagBag Categories { get; set; }

        /// <summary>
        ///   Non-processing information intended to provide a comment to the calendar user.
        /// </summary>
        public MultilingualText Comment { get; set; }

        /// <summary>
        ///   Contact information (textual only).
        /// </summary>
        /// <seealso cref="Attendees"/>
        public List<Contact> Contacts { get; set; }

        /// <summary>
        ///   The status(es) of a scheduling request.
        /// </summary>
        public List<RequestStatus> RequestStatuses { get; set; }

        /// <summary>
        ///   Represents a relationship or reference between this calendar component and
        ///   other calendar components.
        /// </summary>
        public List<RelationshipReference> Relationships { get; set; }

        /// <summary>
        ///   The equipment or resources anticipated for the to-do.
        /// </summary>
        public List<Text> Resources { get; set; }

        /// <summary>
        ///   A reminder or <see cref="VAlarm">alarm</see>> for the event.
        /// </summary>
        public VAlarm Alarm { get; set; }

        // TODO: Completed
        // TODO: Percent

        /// <inheritdoc/>
        public void ReadIcs(IcsReader reader)
        {
            ContentLine content;
            while (null != (content = reader.ReadContentLine()))
            {
                switch (content.Name.ToLowerInvariant())
                {
                    case "end":
                        if (!content.Value.Equals(Component.Names.Todo, StringComparison.InvariantCultureIgnoreCase))
                            throw new CalendarException(String.Format("Expected 'END:{0}' not '{1}'.", Component.Names.Todo, content));
                        return;
                    case "begin":
                        switch (content.Value.ToUpperInvariant())
                        {
                            case Component.Names.Alarm:
                                Alarm = new VAlarm();
                                Alarm.ReadIcs(reader);
                                break;
                            default:
                                throw new CalendarException(string.Format("'{0}' is not a component of a todo.", content.Value));
                        }
                        break;
                    case "attach": Attachments.Add(new CalendarAttachment(content)); break;
                    case "class": Classification = content.ToTag<AccessClassification>(); break;
                    case "comment": Comment.Add(content.ToText()); break;
                    case "created": CreatedOn = content.ToDate(); break;
                    case "dtstamp": CreatedOnByAgent = content.ToDate(); break;
                    case "description": Description = content.ToText(); break;
                    case "duration": Duration = content.ToTimeSpan(); break;
                    case "due": DueOn = content.ToDate(); break;
                    case "geo": GlobalPosition = content.ToGeoCoordinate(); break;
                    case "uid": Id = content.Value; break;
                    case "location": Location = content.ToText(); break;
                    case "last-modified": ModifiedOn = content.ToDate(); break;
                    case "organizer": Organizer = content.ToMailAddress(); break;
                    case "exdate": ExcludedDates.AddRange(content.ToRecurrenceDates()); break;
                    case "rdate": RecurrenceDates.AddRange(content.ToRecurrenceDates()); break;
                    case "rrule": RecurrenceRule = RecurrenceRule.Parse(content.Value); break;
                    case "sequence": Revision = content.ToInt32(); break;
                    case "dtstart": StartsOn = content.ToDate(); break;
                    case "recurrence-id": RecurrenceId = content.ToDate(); break;
                    case "summary": Summary = content.ToText(); break;
                    case "status": Status = content.ToTag<TodoStatus>(); break;
                    case "url": Uri = content.Value; break;
                    case "categories": Categories.AddRange(content.ToTags<Tag>()); break;
                    case "request-status": RequestStatuses.Add(new RequestStatus(content)); break;
                    case "attendee": Attendees.Add(new Attendee(content)); break;
                    case "contact": Contacts.Add(new Contact(content)); break;
                    case "resources": Resources.AddRange(content.ToTextEnumerable()); break;
                    case "related-to": Relationships.Add(new RelationshipReference(content)); break;
                    case "priority": Priority = content.ToInt32(); break;
                }
            }

            throw new CalendarException("Unexpected end of file.");
        }
        

        /// <inheritdoc/>
        public void WriteIcs(IcsWriter ics)
        {
            Guard.IsNotNull(ics, "ics");

            ics.WriteBeginComponent(Component.Names.Todo);
            ics.WriteContent("organizer", Organizer);
            ics.WriteContent(Attendees);
            ics.WriteContent("class", Classification.Name);
            ics.WriteContent("comment", Comment);
            ics.WriteContent("created", CreatedOn);
            ics.WriteContent("dtstamp", CreatedOnByAgent);
            ics.WriteContent("description", Description);
            ics.WriteContent("duration", Duration);
            ics.WriteContent("due", DueOn);
            ics.WriteContent("geo", GlobalPosition);
            ics.WriteContent("uid", Id);
            ics.WriteContent("location", Location);
            ics.WriteContent("last-modified", ModifiedOn);
            ics.WriteContent("exdate", ExcludedDates);
            ics.WriteContent(RecurrenceRule);
            if (Revision > 0)
                ics.WriteContent("sequence", Revision);
            ics.WriteContent("recurrence-id", RecurrenceId);
            ics.WriteContent("dtstart", StartsOn);
            ics.WriteContent("summary", Summary);
            if (Status != null)
                ics.WriteContent("status", Status.Name);
            ics.WriteContent("url", Uri);
            ics.WriteContent("categories", Categories == null ? null : Categories.Select(c => c.Name));
            ics.WriteContent(RequestStatuses);
            ics.WriteContent(Contacts);
            ics.WriteContent("resources", Resources);
            ics.WriteContent("rdate", RecurrenceDates);
            ics.WriteContent(Attachments);
            ics.WriteContent(Relationships);
            if (Priority != 0)
                ics.WriteContent("priority", Priority);
            ics.WriteEndComponent();
        }
    
    }
}
