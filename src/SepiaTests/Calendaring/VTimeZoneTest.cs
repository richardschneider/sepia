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
    ///   Unit tests for <see cref="VTimeZone"/>.
    /// </summary>
    [TestClass]
    public class VTimeZoneTest
    {
        const string Crlf = "\r\n";
        static string Simple =
            "BEGIN:VCALENDAR" + Crlf +
            "PRODID:Zimbra-Calendar-Provider" + Crlf +
            "VERSION:2.0" + Crlf +
            "METHOD:REQUEST" + Crlf +
            "BEGIN:VTIMEZONE" + Crlf +
            "TZID:Pacific/Wellington" + Crlf +
            "LAST-MODIFIED:20050809T050000Z" + Crlf + 
            "BEGIN:STANDARD" + Crlf +
            "DTSTART:19710101T030000" + Crlf +
            "TZOFFSETTO:+1200" + Crlf +
            "TZOFFSETFROM:+1300" + Crlf +
            "RRULE:FREQ=YEARLY;WKST=MO;INTERVAL=1;BYMONTH=4;BYDAY=1SU" + Crlf +
            "TZNAME:NZST" + Crlf +
            "END:STANDARD" + Crlf +
            "BEGIN:DAYLIGHT" + Crlf +
            "DTSTART:19710101T020000" + Crlf +
            "TZOFFSETTO:+1300" + Crlf +
            "TZOFFSETFROM:+1200" + Crlf +
            "RRULE:FREQ=YEARLY;WKST=MO;INTERVAL=1;BYMONTH=9;BYDAY=-1SU" + Crlf +
            "TZNAME:NZDT" + Crlf +
            "END:DAYLIGHT" + Crlf +
            "END:VTIMEZONE" + Crlf +
            "END:VCALENDAR" + Crlf;


        [TestMethod]
        public void Reading()
        {
            var calendar = new VCalendar();
            calendar.ReadIcs(IcsReader.Create(new StringReader(Simple)));
            var tz = calendar.Components.OfType<VTimeZone>().First();
            Assert.AreEqual("Pacific/Wellington", tz.Id);
            Assert.AreEqual(new DateTime(2005, 8, 9, 5, 0, 0, 0, DateTimeKind.Utc), tz.ModifiedOn.Value.Value); 
            Assert.IsTrue(tz.Adjustments.OfType<StandardChange>().Any());
            Assert.IsTrue(tz.Adjustments.OfType<DaylightChange>().Any());
        }

        [TestMethod]
        public void Writing()
        {
            var c0 = new VCalendar();
            c0.ReadIcs(IcsReader.Create(new StringReader(Simple)));
            var tz0 = c0.Components.OfType<VTimeZone>().First();
            var ics = new StringWriter();
            c0.WriteIcs(IcsWriter.Create(ics));

            var c1 = new VCalendar();
            c1.ReadIcs(IcsReader.Create(new StringReader(ics.ToString())));
            var tz1 = c1.Components.OfType<VTimeZone>().First();
            var standard1 = tz1.Adjustments.OfType<StandardChange>().First();
            var daylight1 = tz1.Adjustments.OfType<DaylightChange>().First();

            Assert.AreEqual(tz0.Id, tz1.Id);
            Assert.AreEqual(tz0.ModifiedOn.Value, tz1.ModifiedOn.Value);
            Assert.AreEqual(tz0.Uri, tz1.Uri);
            Assert.AreEqual(tz0.Adjustments.Count, tz1.Adjustments.Count);
            Assert.IsTrue(tz1.Adjustments.OfType<StandardChange>().Any());
            Assert.IsTrue(tz1.Adjustments.OfType<DaylightChange>().Any());
        }

        [TestMethod]
        public void LocalTz()
        {
            var tz = VTimeZone.FromTimeZoneInfo(TimeZoneInfo.Local);
            Assert.IsNotNull(tz.Id);
            Assert.IsTrue(tz.Adjustments.Any(), "at least 1 adjustment is required.");
        }

        [TestMethod]
        public void TziWithNoAdjustments()
        {
            var tzi = TimeZoneInfo.GetSystemTimeZones()
                .FirstOrDefault(t => t.GetAdjustmentRules().Length == 0);
            if (tzi == null)
            {
                Assert.Inconclusive("missing time zone info w/no adjustments.");
            }
            var tz = VTimeZone.FromTimeZoneInfo(tzi);
            Assert.AreEqual(1, tz.Adjustments.Count);
        }

        [TestMethod]
        public void TziWithFixedDate()
        {
            var tzi = TimeZoneInfo.GetSystemTimeZones()
                .FirstOrDefault(t => t.GetAdjustmentRules().Any(a => a.DaylightTransitionEnd.IsFixedDateRule));
            if (tzi == null)
            {
                Assert.Inconclusive("missing time zone info w/fixed date rule.");
            }
            var tz = VTimeZone.FromTimeZoneInfo(tzi);
            Assert.AreEqual(1, tz.Adjustments.Count);
        }
    }
}
