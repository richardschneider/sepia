using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Sepia.Calendaring
{
    using Sepia.Calendaring.Serialization;

    /// <summary>
    ///   Note(s) associated with a calendar date.
    /// </summary>
    /// <remarks>
    ///  Examples of a journal entry include a daily record of
    ///  a legislative body or a journal entry of individual telephone
    ///  contacts for the day or an ordered list of accomplishments for the
    ///  day.  The <see cref="Attachments"/> property is used to
    ///  associate document(s) with a calendar date.
    /// </remarks>
    public class VJournal : IResolvable, ICalenderComponent
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="VJournal"/> class with the default
        ///   values.
        /// </summary>
        public VJournal()
        {
            Id = Guid.NewGuid().ToString();
            Classification = AccessClassification.Public;
            RecurrenceDates = new List<Date>(0);
            ExcludedDates = new List<Date>(0);
            Categories = new TagBag();
            RequestStatuses = new List<RequestStatus>(0);
            Attendees = new List<Attendee>(0);
            Contacts = new List<Contact>(0);
            Attachments = new List<CalendarAttachment>(0);
            Relationships = new List<RelationshipReference>(0);
            Description = new MultilingualText();
        }

        /// <summary>
        ///   The date of the journal entry.
        /// </summary>
        /// <remarks>
        ///  The <see cref="StartsOn"/>property is used to specify the calendar date with which the
        ///  journal entry is associated.  Generally, it will have a just a date value, but a date/time value
        ///  is also allowed.
        /// </remarks>
        public Date? StartsOn { get; set; }

        /// <summary>
        ///   When the journal entry was first entered into the calendar store.
        /// </summary>
        public Date? CreatedOn { get; set; }

        /// <summary>
        ///   When the journal entry was last revised in the calendar store.
        /// </summary>
        public Date? ModifiedOn { get; set; }

        /// <summary>
        ///   When the journal entry was created by the calendar user agent.
        /// </summary>
        public Date CreatedOnByAgent { get; set; }

        /// <summary>
        ///   The globally unique identifier for the journal entry.
        /// </summary>
        /// <value>
        ///   The default value is <see cref="Guid.NewGuid"/>.
        /// </value>
        public string Id { get; set; }

        /// <inheritdoc/>
        public string Uri { get; set; }

        /// <summary>
        ///   The revision sequence number of the journal entry.
        /// </summary>
        /// <remarks>
        ///   When an journal is created, its revision is 0.  It is monotonically incremented by the "Organizer's"
        ///   CUA each time the "Organizer" makes a significant revision to the journal entry.
        /// </remarks>
        public int Revision { get; set; }

        /// <summary>
        ///   The access classification for the journal entry.
        /// </summary>
        ///   The default value is <see cref="AccessClassification.Public"/>.
        /// <remarks>
        ///   Provides a method of capturing the scope of the access the calendar owner intends for journal information.
        ///  <para>
        ///   Applications MUST treat tags values they don't recognize the same way as they would the 
        ///   <see cref="AccessClassification.Private"/> value.
        ///  </para> 
        /// </remarks>
        public AccessClassification Classification { get; set; }

        /// <summary>
        ///   The person who is coordinating the journal entry.
        /// </summary>
        public MailAddress Organizer { get; set; }

        /// <summary>
        ///   The overall status or confirmation of the journal entry.
        /// </summary>
        public JournalStatus Status { get; set; } 

        /// <summary>
        ///   A short summary or subject for the journal entry.
        /// </summary>
        public Text Summary { get; set; }

        /// <summary>
        ///   A more complete description of the journal entry than that provided by <see cref="Summary"/>.
        /// </summary>
        public MultilingualText Description { get; set; } 

        /// <summary>
        ///   Any document (resource) associated with the calendar component.
        /// </summary>
        public List<CalendarAttachment> Attachments { get; set; }

        /// <summary>
        ///   The participants of the journal entry.
        /// </summary>
        public List<Attendee> Attendees { get; set; }

        /// <summary>
        ///   Contact information (textual only).
        /// </summary>
        /// <seealso cref="Attendees"/>
        public List<Contact> Contacts { get; set; }

        /// <summary>
        ///   The categories or subtypes of the calendar component.
        /// </summary>
        public TagBag Categories { get; set; }

        /// <summary>
        ///   Non-processing information intended to provide a comment to the calendar user.
        /// </summary>
        public MultilingualText Comment { get; set; } 

        /// <summary>
        ///   The date time(s) excluded from the resultant set.
        /// </summary>
        public List<Date> ExcludedDates { get; set; }

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
        ///   The rule or repeating pattern for a recurring journal entry.
        /// </summary>
        /// <seealso cref="RecurrenceDates"/>
        public RecurrenceRule RecurrenceRule { get; set; }

        /// <summary>
        ///   Represents a relationship or reference between this calendar component and
        ///   other calendar components.
        /// </summary>
        public List<RelationshipReference> Relationships { get; set; }

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
                        if (!content.Value.Equals(Component.Names.Journal, StringComparison.InvariantCultureIgnoreCase))
                            throw new CalendarException(String.Format("Expected 'END:{0}' not '{1}'.", Component.Names.Journal, content));
                        return;
                    case "class":
                        Classification = content.ToTag<AccessClassification>();
                        break;
                    case "comment":
                        Comment.Add(content.ToText());
                        break;
                    case "created":
                        CreatedOn = content.ToDate();
                        break;
                    case "dtstamp":
                        CreatedOnByAgent = content.ToDate();
                        break;
                    case "description":
                        Description.Add(content.ToText());
                        break;
                    case "uid":
                        Id = content.Value;
                        break;
                    case "last-modified":
                        ModifiedOn = content.ToDate();
                        break;
                    case "organizer":
                        Organizer = content.ToMailAddress();
                        break;
                    case "exdate":
                        ExcludedDates.AddRange(content.ToRecurrenceDates());
                        break;
                    case "recurrence-id": 
                        RecurrenceId = content.ToDate(); 
                        break;
                    case "rdate":
                        RecurrenceDates.AddRange(content.ToRecurrenceDates());
                        break;
                    case "rrule":
                        RecurrenceRule = RecurrenceRule.Parse(content.Value);
                        break;
                    case "sequence":
                        Revision = content.ToInt32();
                        break;
                    case "dtstart":
                        StartsOn = content.ToDate();
                        break;
                    case "summary":
                        Summary = content.ToText();
                        break;
                    case "status":
                        Status = content.ToTag<JournalStatus>();
                        break;
                    case "url":
                        Uri = content.Value;
                        break;
                    case "categories":
                        Categories.AddRange(content.ToTags<Tag>());
                        break;
                    case "request-status":
                        RequestStatuses.Add(new RequestStatus(content));
                        break;
                    case "attendee":
                        Attendees.Add(new Attendee(content));
                        break;
                    case "contact":
                        Contacts.Add(new Contact(content));
                        break;
                    case "attach": 
                        Attachments.Add(new CalendarAttachment(content)); 
                        break;
                    case "related-to": 
                        Relationships.Add(new RelationshipReference(content)); 
                        break;
                }
            }
        }

        /// <inheritdoc/>
        public void WriteIcs(IcsWriter ics)
        {
            Guard.IsNotNull(ics, "ics");

            ics.WriteBeginComponent(Component.Names.Journal);
            ics.WriteContent("organizer", Organizer);
            ics.WriteContent(Attendees);
            ics.WriteContent("class", Classification.Name);
            ics.WriteContent("comment", Comment);
            ics.WriteContent("created", CreatedOn);
            ics.WriteContent("dtstamp", CreatedOnByAgent);
            ics.WriteContent("description", Description);
            ics.WriteContent("uid", Id);
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
            if (Categories.Count > 0)
                ics.WriteContent("categories", Categories.Select(c => c.Name));
            ics.WriteContent(RequestStatuses);
            ics.WriteContent(Contacts);
            ics.WriteContent("rdate", RecurrenceDates);
            ics.WriteContent(Attachments);
            ics.WriteContent(Relationships);
            ics.WriteEndComponent();
        }
    }
}
