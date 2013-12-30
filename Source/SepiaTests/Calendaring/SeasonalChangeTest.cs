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
    ///   Unit tests for <see cref="SeasonalChange"/>.
    /// </summary>
    [TestClass]
    public class SeasonChangeTest
    {
        const string Crlf = "\r\n";
        static string Simple =
            "BEGIN:VCALENDAR" + Crlf +
            "PRODID:Zimbra-Calendar-Provider" + Crlf +
            "VERSION:2.0" + Crlf +
            "METHOD:REQUEST" + Crlf +
            "BEGIN:VTIMEZONE" + Crlf +
            "TZID:Pacific/Wellington" + Crlf +
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
            "COMMENT;LANGUAGE=en-AU:'Bout time mate" + Crlf +
            "COMMENT;LANGUAGE=en-NZ:Sweet as" + Crlf +
            "END:DAYLIGHT" + Crlf +
            "END:VTIMEZONE" + Crlf +
            "END:VCALENDAR" + Crlf;


        [TestMethod]
        public void Reading()
        {
            var calendar = new VCalendar();
            calendar.ReadIcs(IcsReader.Create(new StringReader(Simple)));
            var tz = calendar.Components.OfType<VTimeZone>().First();

            var winter = tz.Adjustments.OfType<StandardChange>().First();
            Assert.AreEqual("NZST", winter.Name);
            Assert.AreEqual(new DateTime(1971, 01, 01, 03, 00, 00), winter.StartsOn);
            Assert.AreEqual(new TimeSpan(12, 0, 0), winter.OffsetTo);
            Assert.AreEqual(new TimeSpan(13, 0, 0), winter.OffsetFrom);

            var summer = tz.Adjustments.OfType<DaylightChange>().First();
            Assert.AreEqual("NZDT", summer.Name);
            Assert.AreEqual(new DateTime(1971, 01, 01, 02, 00, 00), summer.StartsOn);
            Assert.AreEqual("'Bout time mate", summer.Comment["en-AU"].Value);
            Assert.AreEqual("Sweet as", summer.Comment["en-NZ"].Value);
        }

        [TestMethod]
        public void Writting()
        {
            var c0 = new VCalendar();
            c0.ReadIcs(IcsReader.Create(new StringReader(Simple)));
            var tz0 = c0.Components.OfType<VTimeZone>().First();
            var ics = new StringWriter();
            c0.WriteIcs(IcsWriter.Create(ics));

            var c1 = new VCalendar();
            c1.ReadIcs(IcsReader.Create(new StringReader(ics.ToString())));
            var tz1 = c1.Components.OfType<VTimeZone>().First();

            var winter = tz1.Adjustments.OfType<StandardChange>().First();
            Assert.AreEqual("NZST", winter.Name);
            Assert.AreEqual(new DateTime(1971, 01, 01, 03, 00, 00), winter.StartsOn);
            Assert.AreEqual(new TimeSpan(12, 0, 0), winter.OffsetTo);
            Assert.AreEqual(new TimeSpan(13, 0, 0), winter.OffsetFrom);

            var summer = tz1.Adjustments.OfType<DaylightChange>().First();
            Assert.AreEqual("NZDT", summer.Name);
            Assert.AreEqual(new DateTime(1971, 01, 01, 02, 00, 00), summer.StartsOn);
            Assert.AreEqual("'Bout time mate", summer.Comment["en-AU"].Value);
            Assert.AreEqual("Sweet as", summer.Comment["en-NZ"].Value);
        }
    }
}
