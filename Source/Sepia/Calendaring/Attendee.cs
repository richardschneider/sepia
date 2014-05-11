using Sepia.Calendaring.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   A participant, non-participant or the chair of a group-scheduled calendar entity.
    /// </summary>
    public class Attendee : IcsSerializable
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="Attendee"/> class.
        /// </summary>
        public Attendee()
        {
            Membership = new List<MailAddress>(0);
            DelegatedTo = new List<MailAddress>(0);
            DelegatedFrom = new List<MailAddress>(0);
        }

        /// <summary>
        ///   Creates a new instance of the <see cref="Attendee"/> class from
        ///   the specified <see cref="ContentLine"/>.
        /// </summary>
        /// <param name="content">
        ///   The <see cref="ContentLine"/> containing the attendee.
        /// </param>
        public Attendee(ContentLine content) : this()
        {
            Guard.IsNotNull(content, "content");
            Guard.Require(content.Name.Equals("ATTENDEE", StringComparison.InvariantCultureIgnoreCase), "content", "Expected an ATTENDEE content line.");

            MailAddress = content.ToMailAddress();
            if (content.HasParameters)
            {
                Membership.AddRange(ToMailAddresses(content.Parameters.GetValues("MEMBER")));
                DelegatedFrom.AddRange(ToMailAddresses(content.Parameters.GetValues("DELEGATED-FROM")));
                DelegatedTo.AddRange(ToMailAddresses(content.Parameters.GetValues("DELEGATED-TO")));
                SentBy = ToMailAddresses(content.Parameters.GetValues("SENT-BY")).FirstOrDefault();
                var name = content.Parameters["ROLE"];
                if (name != null)
                    UserRole = new UserRole { Name = name };
                name = content.Parameters["CUTYPE"];
                if (name != null)
                    UserType = new UserType { Name = name };
                name = content.Parameters["PARTSTAT"];
                if (name != null)
                    ParticipationStatus = new ParticipationStatus { Name = name };
                name = content.Parameters["RSVP"];
                if (name != null)
                    RsvpExpectation = name.Equals("true", StringComparison.InvariantCultureIgnoreCase);
            }
        }

        static IEnumerable<MailAddress> ToMailAddresses(IEnumerable<string> uris)
        {
           if (uris != null)
           {
               foreach (var uri in uris)
               {
                   if (!uri.StartsWith("mailto:", StringComparison.InvariantCultureIgnoreCase))
                       throw new CalendarException("Expected a mailto URI.");

                   yield return new MailAddress(uri.Substring(7));
               }
           }
        }

        /// <summary>
        ///   The email address of the calendar user.
        /// </summary>
        public MailAddress MailAddress { get; set; }

        /// <summary>
        ///   Identifies the type of the calendar user.
        /// </summary>
        public UserType UserType { get; set; }

        /// <summary>
        ///   Identifies the participation role the calendar user.
        /// </summary>
        public UserRole UserRole { get; set; }

        /// <summary>
        ///   The group membership(s) of the the calendar user.
        /// </summary>
        public List<MailAddress> Membership { get; set; }

        /// <summary>
        ///   Identifies the participation status of the calendar user.
        /// </summary>
        public ParticipationStatus ParticipationStatus { get; set; }

        /// <summary>
        ///   Determines if an expectation of a favor of a reply from the calendar user
        /// </summary>
        public bool RsvpExpectation { get; set; }

        /// <summary>
        ///   The calendar users to whom have delegated participation.
        /// </summary>
        public List<MailAddress> DelegatedTo { get; set; }

        /// <summary>
        ///   The calendar users that have delegated their participation to the calendar user.
        /// </summary>
        public List<MailAddress> DelegatedFrom { get; set; }

        /// <summary>
        ///   The calendar user that is acting on behalf of another calendar user.
        /// </summary>
        public MailAddress SentBy { get; set; }

        /// <inheritdoc />
        public void ReadIcs(IcsReader reader) // TODO
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void WriteIcs(IcsWriter writer)
        {
            var content = new ContentLine { Name = "attendee", Value = "mailto:" + MailAddress.Address };
            if (!string.IsNullOrWhiteSpace(MailAddress.DisplayName))
                content.Parameters[ParameterName.CommonName] = MailAddress.DisplayName;
            foreach (var mail in Membership)
            {
                content.Parameters.Add("member", "mailto:" + mail.Address);
            }
            foreach (var mail in DelegatedTo)
            {
                content.Parameters.Add("delegated-to", "mailto:" + mail.Address);
            }
            foreach (var mail in DelegatedFrom)
            {
                content.Parameters.Add("delegated-from", "mailto:" + mail.Address);
            }
            if (SentBy != null)
                content.Parameters.Add("sent-by", "mailto:" + SentBy.Address);
            if (UserRole != null)
                content.Parameters.Add("role", UserRole.Name);
            if (UserType != null)
                content.Parameters.Add("cutype", UserType.Name);
            if (ParticipationStatus != null)
                content.Parameters.Add("partstat", ParticipationStatus.Name);
            if (RsvpExpectation)
                content.Parameters.Add("rsvp", "TRUE");
            writer.Write(content);
        }
    }
}
