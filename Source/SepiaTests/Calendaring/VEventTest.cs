using Sepia.Calendaring.Serialization;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia.Calendaring
{
    /// <summary>
    ///   Unit tests for <see cref="VEvent"/>.
    /// </summary>
    [TestClass]
    public class VEventTest
    {
        const string Crlf = "\r\n";
        static string Simple =
           "BEGIN:VCALENDAR" + Crlf +
           "VERSION:2.0" + Crlf +
           "PRODID:-//hacksw/handcal//NONSGML v1.0//EN" + Crlf +
           "BEGIN:VEVENT" + Crlf +
           "UID:19970610T172345Z-AF23B2@example.com" + Crlf +
           "DTSTAMP:19970610T172345Z" + Crlf +
           "DTSTART:19970714T170000Z" + Crlf +
           "DTEND:19970715T040000Z" + Crlf +
           "SUMMARY:Bastille Day Party" + Crlf +
           "CATEGORIES:APPOINTMENT,EDUCATION" + Crlf +
           "RESOURCES:EASEL,PROJECTOR,VCR" + Crlf +
           "RESOURCES;LANGUAGE=fr:Nettoyeur haute pression" + Crlf +
           "END:VEVENT" + Crlf +
           "END:VCALENDAR" + Crlf;

        static string SimpleDuration =
           "BEGIN:VCALENDAR" + Crlf +
           "VERSION:2.0" + Crlf +
           "PRODID:-//hacksw/handcal//NONSGML v1.0//EN" + Crlf +
           "BEGIN:VEVENT" + Crlf +
           "UID:19970610T172345Z-AF23B2@example.com" + Crlf +
           "DTSTAMP:19970610T172345Z" + Crlf +
           "DTSTART:19970714T170000Z" + Crlf +
           "DURATION:PT3H" + Crlf +
           "SUMMARY:Bastille Day Party" + Crlf +
           "CATEGORIES:APPOINTMENT,EDUCATION" + Crlf +
           "RESOURCES:EASEL,PROJECTOR,VCR" + Crlf +
           "RESOURCES;LANGUAGE=fr:Nettoyeur haute pression" + Crlf +
           "END:VEVENT" + Crlf +
           "END:VCALENDAR" + Crlf;

        static string Full =
           "BEGIN:VCALENDAR" + Crlf +
               "VERSION:2.0" + Crlf +
               "PRODID:-//hacksw/handcal//NONSGML v1.0//EN" + Crlf +
           "BEGIN:VEVENT" + Crlf +
               "UID:19970610T172345Z-AF23B2@example.com" + Crlf +
               "DTSTAMP:19970610T172345Z" + Crlf +
               "DTSTART:19970714T170000Z" + Crlf +
               "DTEND:19970715T040000Z" + Crlf +
               "SUMMARY:Bastille Day Party" + Crlf +
               "ORGANIZER;CN=John Smith:mailto:jsmith@example.com" + Crlf +
               "ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=TENTATIVE;CN=Henry Cabot:mailto:hcabot@example.com" + Crlf +
               "CATEGORIES:APPOINTMENT,EDUCATION" + Crlf +
               "RESOURCES:EASEL,PROJECTOR,VCR" + Crlf +
               "RESOURCES;LANGUAGE=fr:Nettoyeur haute pression" + Crlf +
               "SEQUENCE:2" + Crlf +
               "STATUS:TENTATIVE" + Crlf +
               "TRANSP:TRANSPARENT" + Crlf +
               "GEO:37.386013;-122.082932" + Crlf +
               "CONTACT;ALTREP=\"http://host.com/pdi/jdoe.vcf\":Jim" + Crlf +
               "  Dolittle\\, ABC Industries\\, +1-919-555-1234" + Crlf +
               "ATTACH;FMTTYPE=audio/basic:ftp://example.com/pub/sounds/bell-01.aud" + Crlf +
               "ATTACH;FMTTYPE=application/msword:http://example.com/" + Crlf +
               " templates/agenda.doc" + Crlf +
               "RELATED-TO:jsmith.part7.19960817T083000.xyzMail@example.com" + Crlf +
               "REQUEST-STATUS:2.8; Success\\, repeating event ignored. Scheduled " + Crlf +
               " as a single event.;RRULE:FREQ=WEEKLY\\;INTERVAL=2" + Crlf +
               "BEGIN:VALARM" + Crlf +
                "TRIGGER;VALUE=DATE-TIME:19970317T133000Z" + Crlf +
                "REPEAT:4" + Crlf +
                "DURATION:PT15M" + Crlf +
                "ACTION:AUDIO" + Crlf +
                "ATTACH;FMTTYPE=audio/basic:ftp://example.com/pub/sounds/bell-01.aud" + Crlf +
            "END:VALARM" + Crlf +
            "END:VEVENT" + Crlf +
            "END:VCALENDAR" + Crlf;

        /// <summary>
        ///   The default values for properties are defined when an instance is created.
        /// </summary>
        [TestMethod]
        public void Defaults()
        {
            var now = DateTimeOffset.Now;
            var e0 = new VEvent();
            var e1 = new VEvent();
            Assert.AreNotEqual(e0.Id, e1.Id, "id is not unique");
            Assert.IsFalse(e0.StartsOn.HasValue, "starts on");
            Assert.AreEqual(AccessClassification.Public, e0.Classification);
            Assert.AreEqual(0, e0.Priority, "undefined priority");
            Assert.AreEqual(0, e0.Revision);
            Assert.IsFalse(e0.IsTransparent, "opaque");
        }

        /// <summary>
        ///   An email address can be used as an organiser.
        /// </summary>
        [TestMethod]
        public void MailAddressForOrganiser()
        {
            var meeting = new VEvent();
            meeting.Organizer = new MailAddress("jane.doe@somewhere.com", "Jane Doe");
        }

        [TestMethod]
        public void Reading()
        {
            var calendar = new VCalendar();
            calendar.ReadIcs(IcsReader.Create(new StringReader(Simple)));
            var meeting = calendar.Components.OfType<VEvent>().First();
            Assert.AreEqual("19970610T172345Z-AF23B2@example.com", meeting.Id);
            Assert.AreEqual("Bastille Day Party", meeting.Summary);
            Assert.AreEqual(LanguageTag.Unspecified, meeting.Summary.Language);
            Assert.AreEqual(new DateTime(1997, 7, 14, 17, 0, 0, DateTimeKind.Utc), meeting.StartsOn.Value.Value);
            Assert.AreEqual(new DateTime(1997, 7, 15, 4, 0, 0, DateTimeKind.Utc), meeting.EndsOn.Value.Value);
            Assert.IsTrue(meeting.Categories.Any(c => c.Name == "EDUCATION"));
            Assert.IsTrue(meeting.Categories.Any(c => c.Name == "APPOINTMENT"));
            Assert.IsFalse(meeting.Categories.Any(c => c.Name == "PERSONAL"));
            Assert.IsTrue(meeting.Resources.Any(r => r.Value == "EASEL"));
            Assert.IsTrue(meeting.Resources.Any(r => r.Value == "PROJECTOR"));
            Assert.IsTrue(meeting.Resources.Any(r => r.Value == "VCR"));
            Assert.IsTrue(meeting.Resources.Any(r => r.Value == "Nettoyeur haute pression" && r.Language.Name == "fr"));
            Assert.IsNull(meeting.Alarm, "no alarm");
        }

        [TestMethod]
        public void ReadingDuration()
        {
            var calendar = new VCalendar();
            calendar.ReadIcs(IcsReader.Create(new StringReader(SimpleDuration)));
            var meeting = calendar.Components.OfType<VEvent>().First();
            Assert.AreEqual("19970610T172345Z-AF23B2@example.com", meeting.Id);
            Assert.AreEqual("Bastille Day Party", meeting.Summary);
            Assert.AreEqual(LanguageTag.Unspecified, meeting.Summary.Language);
            Assert.AreEqual(new DateTime(1997, 7, 14, 17, 0, 0, DateTimeKind.Utc), meeting.StartsOn.Value.Value);
            Assert.IsTrue(meeting.Duration.HasValue);
            Assert.AreEqual(new TimeSpan(3, 0, 0), meeting.Duration);
        }

        [TestMethod]
        public void Writing()
        {
            var calendar0 = new VCalendar();
            calendar0.ReadIcs(IcsReader.Create(new StringReader(Full)));
            var ics = new StringWriter();
            calendar0.WriteIcs(IcsWriter.Create(ics));
            //Console.WriteLine(ics.ToString());
            var calendar1 = new VCalendar();
            calendar1.ReadIcs(IcsReader.Create(new StringReader(ics.ToString())));

            var meeting = calendar1.Components.OfType<VEvent>().First();
            Assert.AreEqual("19970610T172345Z-AF23B2@example.com", meeting.Id);
            Assert.AreEqual("Bastille Day Party", meeting.Summary);
            Assert.AreEqual(LanguageTag.Unspecified, meeting.Summary.Language);
            Assert.AreEqual(new DateTime(1997, 7, 14, 17, 0, 0, DateTimeKind.Utc), meeting.StartsOn.Value.Value);
            Assert.AreEqual(new DateTime(1997, 7, 15, 4, 0, 0, DateTimeKind.Utc), meeting.EndsOn.Value.Value);
            Assert.IsTrue(meeting.Categories.Any(c => c.Name == "EDUCATION"));
            Assert.IsTrue(meeting.Categories.Any(c => c.Name == "APPOINTMENT"));
            Assert.IsFalse(meeting.Categories.Any(c => c.Name == "PERSONAL"));
            Assert.IsTrue(meeting.Resources.Any(r => r.Value == "EASEL"));
            Assert.IsTrue(meeting.Resources.Any(r => r.Value == "PROJECTOR"));
            Assert.IsTrue(meeting.Resources.Any(r => r.Value == "VCR"));
            Assert.IsTrue(meeting.Resources.Any(r => r.Value == "Nettoyeur haute pression" && r.Language.Name == "fr"));
            Assert.AreEqual("John Smith", meeting.Organizer.DisplayName);
            Assert.AreEqual("jsmith@example.com", meeting.Organizer.Address);
            Assert.IsNotNull(meeting.Alarm, "missing alarm");
            Assert.AreEqual(1, meeting.Attendees.Count, "missing the attendee");
            Assert.AreEqual(2, meeting.Revision);
            Assert.AreEqual(EventStatus.Tentative, meeting.Status);
            Assert.IsTrue(meeting.IsTransparent);
            Assert.AreEqual(37.386013d, meeting.GlobalPosition.Latitude);
            Assert.AreEqual(-122.082932d, meeting.GlobalPosition.Longitude);
            Assert.AreEqual(1, meeting.Contacts.Count, "missing contact");
            Assert.AreEqual("http://host.com/pdi/jdoe.vcf", meeting.Contacts[0].Uri);
            Assert.AreEqual("Jim Dolittle, ABC Industries, +1-919-555-1234", meeting.Contacts[0].Text);
            Assert.AreEqual("audio/basic", meeting.Attachments[0].ContentType);
            Assert.AreEqual("ftp://example.com/pub/sounds/bell-01.aud", meeting.Attachments[0].Uri);
            Assert.AreEqual("application/msword", meeting.Attachments[1].ContentType);
            Assert.AreEqual("http://example.com/templates/agenda.doc", meeting.Attachments[1].Uri);
            Assert.AreEqual(1, meeting.Relationships.Count, "missing relationship");
            Assert.AreEqual("jsmith.part7.19960817T083000.xyzMail@example.com", meeting.Relationships[0].OtherUri);
            Assert.AreEqual(1, meeting.RequestStatuses.Count, "missing request statuses");
            Assert.AreEqual("2.8", meeting.RequestStatuses[0].Code);
            Assert.AreEqual("Success, repeating event ignored. Scheduled as a single event.", meeting.RequestStatuses[0].Description);
            Assert.AreEqual(LanguageTag.Unspecified, meeting.RequestStatuses[0].Description.Language);
            Assert.AreEqual("RRULE:FREQ=WEEKLY;INTERVAL=2", meeting.RequestStatuses[0].RelatedData);
            Assert.IsTrue(meeting.RequestStatuses[0].IsSuccess);
        }

        [TestMethod]
        public void ReadingWithAlarm()
        {
            const string ics =
                "BEGIN:VCALENDAR" + Crlf +
                    "VERSION:2.0" + Crlf +
                    "BEGIN:VEVENT" + Crlf +
                        "UID:19970610T172345Z-AF23B2@example.com" + Crlf +
                        "DTSTAMP:19970610T172345Z" + Crlf +
                        "DTSTART:19970714T170000Z" + Crlf +
                        "DTEND:19970715T040000Z" + Crlf +
                        "BEGIN:VALARM" + Crlf +
                            "TRIGGER;VALUE=DATE-TIME:19970317T133000Z" + Crlf +
                            "REPEAT:4" + Crlf +
                            "DURATION:PT15M" + Crlf +
                            "ACTION:AUDIO" + Crlf +
                            "ATTACH;FMTTYPE=audio/basic:ftp://example.com/pub/sounds/bell-01.aud" + Crlf +
                        "END:VALARM" + Crlf +
                    "END:VEVENT" + Crlf +
                "END:VCALENDAR" + Crlf;

            var calendar = new VCalendar();
            calendar.ReadIcs(IcsReader.Create(new StringReader(ics)));
            var meeting = calendar.Components.OfType<VEvent>().First();
            Assert.IsNotNull(meeting.Alarm, "missing alarm");
        }
    }
}
