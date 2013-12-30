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
    ///   Unit tests for <see cref="VFreeBusy"/>.
    /// </summary>
    [TestClass]
    public class VFreeBusyTest
    {
        const string Crlf = "\r\n";

        [TestMethod]
        public void ReadingPublished()
        {
            const string ics =
               "BEGIN:VFREEBUSY" + Crlf +
               "UID:19970901T115957Z-76A912@example.com" + Crlf +
               "DTSTAMP:19970901T120000Z" + Crlf +
               "ORGANIZER:mailto:jsmith@example.com" + Crlf +
               "DTSTART:19980313T141711Z" + Crlf +
               "DTEND:19980410T141711Z" + Crlf +
               "FREEBUSY:19980314T233000Z/19980315T003000Z" + Crlf +
               "FREEBUSY:19980316T153000Z/19980316T163000Z" + Crlf +
               "FREEBUSY:19980318T030000Z/19980318T040000Z" + Crlf +
               "URL:http://www.example.com/calendar/busytime/jsmith.ifb" + Crlf +
               "END:VFREEBUSY" + Crlf;

            var freebusy = new VFreeBusy();
            freebusy.ReadIcs(IcsReader.Create(new StringReader(ics)));
            Assert.AreEqual("19970901T115957Z-76A912@example.com", freebusy.Id);
            Assert.AreEqual(new DateTime(1997, 09, 01, 12, 00, 00, DateTimeKind.Utc), freebusy.CreatedOnByAgent.Value);
            Assert.AreEqual("jsmith@example.com", freebusy.Organizer.Address);
            Assert.AreEqual(new DateTime(1998, 03, 13, 14, 17, 11, DateTimeKind.Utc), freebusy.StartsOn.Value.Value);
            Assert.AreEqual(new DateTime(1998, 04, 10, 14, 17, 11, DateTimeKind.Utc), freebusy.EndsOn.Value.Value);
            Assert.AreEqual(3, freebusy.FreeBusyTimes.Count);
            Assert.AreEqual("http://www.example.com/calendar/busytime/jsmith.ifb", freebusy.Uri);
        }

        [TestMethod]
        public void WritingPublished()
        {
            const string ics0 =
               "BEGIN:VFREEBUSY" + Crlf +
               "UID:19970901T115957Z-76A912@example.com" + Crlf +
               "DTSTAMP:19970901T120000Z" + Crlf +
               "ORGANIZER:mailto:jsmith@example.com" + Crlf +
               "DTSTART:19980313T141711Z" + Crlf +
               "DTEND:19980410T141711Z" + Crlf +
               "FREEBUSY:19980314T233000Z/19980315T003000Z" + Crlf +
               "FREEBUSY:19980316T153000Z/19980316T163000Z" + Crlf +
               "FREEBUSY:19980318T030000Z/19980318T040000Z" + Crlf +
               "URL:http://www.example.com/calendar/busytime/jsmith.ifb" + Crlf +
               "END:VFREEBUSY" + Crlf;

            var freebusy0 = new VFreeBusy();
            freebusy0.ReadIcs(IcsReader.Create(new StringReader(ics0)));
            var ics1 = new StringWriter();
            freebusy0.WriteIcs(IcsWriter.Create(ics1));
            var freebusy = new VFreeBusy();
            freebusy.ReadIcs(IcsReader.Create(new StringReader(ics1.ToString())));

            Assert.AreEqual("19970901T115957Z-76A912@example.com", freebusy.Id);
            Assert.AreEqual(new DateTime(1997, 09, 01, 12, 00, 00, DateTimeKind.Utc), freebusy.CreatedOnByAgent.Value);
            Assert.AreEqual("jsmith@example.com", freebusy.Organizer.Address);
            Assert.AreEqual(new DateTime(1998, 03, 13, 14, 17, 11, DateTimeKind.Utc), freebusy.StartsOn.Value.Value);
            Assert.AreEqual(new DateTime(1998, 04, 10, 14, 17, 11, DateTimeKind.Utc), freebusy.EndsOn.Value.Value);
            Assert.AreEqual(3, freebusy.FreeBusyTimes.Count);
            Assert.AreEqual("http://www.example.com/calendar/busytime/jsmith.ifb", freebusy.Uri);
        }

    }
}
