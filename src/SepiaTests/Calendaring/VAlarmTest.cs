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
    ///   Unit tests for <see cref="VAlarm"/>.
    /// </summary>
    [TestClass]
    public class VAlarmTest
    {
        const string Crlf = "\r\n";

        [TestMethod]
        public void ReadingAudio()
        {
            const string ics =
                "BEGIN:VALARM" + Crlf +
                "TRIGGER;VALUE=DATE-TIME:19970317T133000Z" + Crlf +
                "REPEAT:4" + Crlf +
                "DURATION:PT15M" + Crlf +
                "ACTION:AUDIO" + Crlf +
                "ATTACH;FMTTYPE=audio/basic:ftp://example.com/pub/sounds/bell-01.aud" + Crlf +
                "END:VALARM" + Crlf;

            var alarm = new VAlarm();
            alarm.ReadIcs(IcsReader.Create(new StringReader(ics)));
            Assert.AreEqual(AlarmAction.Audio, alarm.Action);
            Assert.AreEqual(new DateTime(1997, 03, 17, 13, 30, 00, DateTimeKind.Utc), alarm.TriggerOn);
            Assert.AreEqual(new TimeSpan(0, 0, 15, 0), alarm.Duration);
            Assert.AreEqual("audio/basic", alarm.Attachment.ContentType);
            Assert.AreEqual("ftp://example.com/pub/sounds/bell-01.aud", alarm.Attachment.Uri);
            Assert.AreEqual(4, alarm.Repeat);
        }

        [TestMethod]
        public void WritingAudio()
        {
            const string ics0 =
                "BEGIN:VALARM" + Crlf +
                "TRIGGER;VALUE=DATE-TIME:19970317T133000Z" + Crlf +
                "REPEAT:4" + Crlf +
                "DURATION:PT15M" + Crlf +
                "ACTION:AUDIO" + Crlf +
                "ATTACH;FMTTYPE=audio/basic:ftp://example.com/pub/sounds/bell-01.aud" + Crlf +
                "END:VALARM" + Crlf;

            var alarm0 = new VAlarm();
            alarm0.ReadIcs(IcsReader.Create(new StringReader(ics0)));
            var ics1 = new StringWriter();
            alarm0.WriteIcs(IcsWriter.Create(ics1));
            var alarm = new VAlarm();
            alarm.ReadIcs(IcsReader.Create(new StringReader(ics1.ToString())));

            Assert.AreEqual(AlarmAction.Audio, alarm.Action);
            Assert.AreEqual(new DateTime(1997, 03, 17, 13, 30, 00, DateTimeKind.Utc), alarm.TriggerOn.Value);
            Assert.AreEqual(new TimeSpan(0, 0, 15, 0), alarm.Duration);
            Assert.AreEqual("audio/basic", alarm.Attachment.ContentType);
            Assert.AreEqual("ftp://example.com/pub/sounds/bell-01.aud", alarm.Attachment.Uri);
            Assert.AreEqual(4, alarm.Repeat);
        }

        [TestMethod]
        public void ReadingDisplay()
        {
            const string ics =
                "BEGIN:VALARM" + Crlf +
                "TRIGGER:-PT30M" + Crlf +
                "REPEAT:2" + Crlf +
                "DURATION:PT15M" + Crlf +
                "ACTION:DISPLAY" + Crlf +
                @"DESCRIPTION:Breakfast meeting with executive\n" + Crlf +
                " team at 8:30 AM EST." + Crlf +
                "END:VALARM" + Crlf;

            var alarm = new VAlarm();
            alarm.ReadIcs(IcsReader.Create(new StringReader(ics)));
            Assert.AreEqual(AlarmAction.Display, alarm.Action);
            Assert.AreEqual(TimeSpan.FromMinutes(-30), alarm.TriggerDuration);
            Assert.AreEqual(TriggerEdge.Start, alarm.TriggerEdge);
            Assert.AreEqual(new TimeSpan(0, 0, 15, 0), alarm.Duration);
            Assert.AreEqual(2, alarm.Repeat);
            Assert.AreEqual("Breakfast meeting with executive" + Environment.NewLine + "team at 8:30 AM EST.", alarm.Description);
        }

        [TestMethod]
        public void WritingDisplay()
        {
            const string ics0 =
                "BEGIN:VALARM" + Crlf +
                "TRIGGER:-PT30M" + Crlf +
                "REPEAT:2" + Crlf +
                "DURATION:PT15M" + Crlf +
                "ACTION:DISPLAY" + Crlf +
                @"DESCRIPTION:Breakfast meeting with executive\n" + Crlf +
                " team at 8:30 AM EST." + Crlf +
                "END:VALARM" + Crlf;

            var alarm0 = new VAlarm();
            alarm0.ReadIcs(IcsReader.Create(new StringReader(ics0)));
            var ics1 = new StringWriter();
            alarm0.WriteIcs(IcsWriter.Create(ics1));
            var alarm = new VAlarm();
            alarm.ReadIcs(IcsReader.Create(new StringReader(ics1.ToString())));

            Assert.AreEqual(AlarmAction.Display, alarm.Action);
            Assert.AreEqual(TimeSpan.FromMinutes(-30), alarm.TriggerDuration.Value);
            Assert.AreEqual(TriggerEdge.Start, alarm.TriggerEdge);
            Assert.AreEqual(new TimeSpan(0, 0, 15, 0), alarm.Duration);
            Assert.AreEqual(2, alarm.Repeat);
            Assert.AreEqual("Breakfast meeting with executive" + Environment.NewLine + "team at 8:30 AM EST.", alarm.Description);
        }

        [TestMethod]
        public void ReadingEmail()
        {
            const string ics =
                "BEGIN:VALARM" + Crlf +
                "TRIGGER;RELATED=END:-P2D" + Crlf +
                "ACTION:EMAIL" + Crlf +
                "ATTENDEE:mailto:john_doe@example.com" + Crlf +
                "SUMMARY:*** REMINDER: SEND AGENDA FOR WEEKLY STAFF MEETING ***" + Crlf +
                "DESCRIPTION:A draft agenda needs to be sent out to the attendees" + Crlf +
                "  to the weekly managers meeting (MGR-LIST). Attached is a" + Crlf +
                "  pointer the document template for the agenda file." + Crlf +
                "ATTACH;FMTTYPE=application/msword:http://example.com/" + Crlf +
                " templates/agenda.doc" + Crlf +
                "END:VALARM" + Crlf;

            var alarm = new VAlarm();
            alarm.ReadIcs(IcsReader.Create(new StringReader(ics)));
            Assert.AreEqual(TimeSpan.FromDays(-2), alarm.TriggerDuration);
            Assert.AreEqual(TriggerEdge.End, alarm.TriggerEdge);
            Assert.AreEqual(AlarmAction.Email, alarm.Action);
            Assert.AreEqual("john_doe@example.com", alarm.Attendees.First().MailAddress.Address);
            Assert.AreEqual("*** REMINDER: SEND AGENDA FOR WEEKLY STAFF MEETING ***", alarm.Summary);
            Assert.AreEqual("A draft agenda needs to be sent out to the attendees to the weekly managers meeting (MGR-LIST). Attached is a pointer the document template for the agenda file.", alarm.Description);
            Assert.AreEqual("application/msword", alarm.Attachment.ContentType);
            Assert.AreEqual("http://example.com/templates/agenda.doc", alarm.Attachment.Uri);
        }

        [TestMethod]
        public void WritingEmail()
        {
            const string ics0 =
                "BEGIN:VALARM" + Crlf +
                "TRIGGER;RELATED=END:-P2D" + Crlf +
                "ACTION:EMAIL" + Crlf +
                "ATTENDEE:mailto:john_doe@example.com" + Crlf +
                "SUMMARY:*** REMINDER: SEND AGENDA FOR WEEKLY STAFF MEETING ***" + Crlf +
                "DESCRIPTION:A draft agenda needs to be sent out to the attendees" + Crlf +
                "  to the weekly managers meeting (MGR-LIST). Attached is a" + Crlf +
                "  pointer the document template for the agenda file." + Crlf +
                "ATTACH;FMTTYPE=application/msword:http://example.com/" + Crlf +
                " templates/agenda.doc" + Crlf +
                "END:VALARM" + Crlf;

            var alarm0 = new VAlarm();
            alarm0.ReadIcs(IcsReader.Create(new StringReader(ics0)));
            var ics1 = new StringWriter();
            alarm0.WriteIcs(IcsWriter.Create(ics1));
            var alarm = new VAlarm();
            alarm.ReadIcs(IcsReader.Create(new StringReader(ics1.ToString())));

            Assert.AreEqual(TimeSpan.FromDays(-2), alarm.TriggerDuration.Value);
            Assert.AreEqual(TriggerEdge.End, alarm.TriggerEdge);
            Assert.AreEqual(AlarmAction.Email, alarm.Action);
            Assert.AreEqual("john_doe@example.com", alarm.Attendees.First().MailAddress.Address);
            Assert.AreEqual("*** REMINDER: SEND AGENDA FOR WEEKLY STAFF MEETING ***", alarm.Summary);
            Assert.AreEqual("A draft agenda needs to be sent out to the attendees to the weekly managers meeting (MGR-LIST). Attached is a pointer the document template for the agenda file.", alarm.Description);
            Assert.AreEqual("application/msword", alarm.Attachment.ContentType);
            Assert.AreEqual("http://example.com/templates/agenda.doc", alarm.Attachment.Uri);
        }

        [TestMethod]
        public void TriggerOnMustUtcRelative()
        {
            var alarm = new VAlarm();
            ExceptionAssert.Throws<ArgumentException>(() => alarm.TriggerOn = DateTime.Now);
        }
    }
}
