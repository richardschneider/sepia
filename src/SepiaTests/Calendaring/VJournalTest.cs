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
    ///   Unit tests for <see cref="VJournal"/>.
    /// </summary>
    [TestClass]
    public class VJournalTest
    {
        const string Crlf = "\r\n";

        /// <summary>
        ///   The default values for properties are defined when an instance is created.
        /// </summary>
        [TestMethod]
        public void Defaults()
        {
            var j0 = new VJournal();
            var j1 = new VJournal();
            Assert.AreNotEqual(j0.Id, j1.Id, "id is not unique");
            Assert.IsFalse(j0.StartsOn.HasValue, "starts on");
            Assert.AreEqual(AccessClassification.Public, j0.Classification);
            Assert.AreEqual(0, j0.Revision);
        }

        [TestMethod]
        public void Reading()
        {
            const string ics =
                "BEGIN:VCALENDAR" + Crlf +
                    "VERSION:2.0" + Crlf +
                    "BEGIN:VJOURNAL" + Crlf +
                        "UID:19970901T130000Z-123405@example.com" + Crlf +
                        "DTSTAMP:19970901T130000Z" + Crlf +
                        "DTSTART;VALUE=DATE:19970317" + Crlf +
                        "SUMMARY:Staff meeting minutes" + Crlf +
                        @"DESCRIPTION:1. Staff meeting\n2. Meeting with ABC Corp." + Crlf +
                        "ATTACH;FMTTYPE=audio/basic:ftp://example.com/pub/sounds/bell-01.aud" + Crlf +
                        "RELATED-TO:jsmith.part7.19960817T083000.xyzMail@example.com" + Crlf +
                    "END:VJOURNAL" + Crlf +
                "END:VCALENDAR" + Crlf;

            var calendar = new VCalendar();
            calendar.ReadIcs(IcsReader.Create(new StringReader(ics)));
            var journal = calendar.Components.OfType<VJournal>().First();
            Assert.AreEqual("19970901T130000Z-123405@example.com", journal.Id);
            Assert.AreEqual(new DateTime(1997, 09, 01, 13, 00, 00, DateTimeKind.Utc), journal.CreatedOnByAgent.Value);
            Assert.AreEqual(new DateTime(1997, 03, 17), journal.StartsOn.Value.Value);
            Assert.AreEqual("Staff meeting minutes", journal.Summary);
            Assert.AreEqual("1. Staff meeting" + Environment.NewLine + "2. Meeting with ABC Corp.", journal.Description[0]);
            Assert.AreEqual("audio/basic", journal.Attachments[0].ContentType);
            Assert.AreEqual("ftp://example.com/pub/sounds/bell-01.aud", journal.Attachments[0].Uri);
            Assert.AreEqual(1, journal.Relationships.Count, "missing relationship");
            Assert.AreEqual("jsmith.part7.19960817T083000.xyzMail@example.com", journal.Relationships[0].OtherUri);
        }

        [TestMethod]
        public void ReadingMultilingualText()
        {
            const string ics =
                "BEGIN:VCALENDAR" + Crlf +
                    "VERSION:2.0" + Crlf +
                    "BEGIN:VJOURNAL" + Crlf +
                        "UID:19970901T130000Z-123405@example.com" + Crlf +
                        "DTSTAMP:19970901T130000Z" + Crlf +
                        "SUMMARY:hello" + Crlf +
                        "DESCRIPTION:hello" + Crlf +
                        "DESCRIPTION;LANGUAGE=en:hi" + Crlf +
                        "DESCRIPTION;LANGUAGE=en-nz:cheers" + Crlf +
                    "END:VJOURNAL" + Crlf +
                "END:VCALENDAR" + Crlf;

            var calendar = new VCalendar();
            calendar.ReadIcs(IcsReader.Create(new StringReader(ics)));
            var journal = calendar.Components.OfType<VJournal>().First();
            Assert.AreEqual("hi", journal.Description.WrittenIn("en"));
            Assert.AreEqual("cheers", journal.Description.WrittenIn("en-nz"));
            Assert.AreEqual("hello", journal.Description.WrittenIn(LanguageTag.Unspecified));
        }

        [TestMethod]
        public void Writing()
        {
            const string ics0 =
                "BEGIN:VJOURNAL" + Crlf +
                    "UID:19970901T130000Z-123405@example.com" + Crlf +
                    "DTSTAMP:19970901T130000Z" + Crlf +
                    "DTSTART;VALUE=DATE:19970317" + Crlf +
                    "SUMMARY:Staff meeting minutes" + Crlf +
                    @"DESCRIPTION:1. Staff meeting\n2. Meeting with ABC Corp." + Crlf +
                    "ATTACH;FMTTYPE=audio/basic:ftp://example.com/pub/sounds/bell-01.aud" + Crlf +
                    "RELATED-TO:jsmith.part7.19960817T083000.xyzMail@example.com" + Crlf +
                "END:VJOURNAL" + Crlf;

            var journal0 = new VJournal();
            journal0.ReadIcs(IcsReader.Create(new StringReader(ics0)));
            var ics1 = new StringWriter();
            journal0.WriteIcs(IcsWriter.Create(ics1));
            var journal = new VJournal();
            journal.ReadIcs(IcsReader.Create(new StringReader(ics1.ToString())));

            Assert.AreEqual("19970901T130000Z-123405@example.com", journal.Id);
            Assert.AreEqual(new DateTime(1997, 09, 01, 13, 00, 00, DateTimeKind.Utc), journal.CreatedOnByAgent.Value);
            Assert.AreEqual(new DateTime(1997, 03, 17), journal.StartsOn.Value.Value);
            Assert.AreEqual("Staff meeting minutes", journal.Summary);
            Assert.AreEqual("1. Staff meeting" + Environment.NewLine + "2. Meeting with ABC Corp.", journal.Description[0]);
            Assert.AreEqual("audio/basic", journal.Attachments[0].ContentType);
            Assert.AreEqual("ftp://example.com/pub/sounds/bell-01.aud", journal.Attachments[0].Uri);
            Assert.AreEqual(1, journal.Relationships.Count, "missing relationship");
            Assert.AreEqual("jsmith.part7.19960817T083000.xyzMail@example.com", journal.Relationships[0].OtherUri);
        }

    }
}
