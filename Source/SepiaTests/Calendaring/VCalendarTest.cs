using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia.Calendaring
{
    using System.IO;
    using Sepia.Calendaring.Serialization;

    /// <summary>
    ///   Unit tests for <see cref="VCalendar"/>.
    /// </summary>
    [TestClass]
    public class VCalendarTest
    {
        const string Crlf = "\r\n";
        static readonly string Simple =
            "BEGIN:VCALENDAR" + Crlf +
            "PRODID:Zimbra-Calendar-Provider" + Crlf +
            "VERSION:2.0" + Crlf +
            "METHOD:REQUEST" + Crlf +
            "END:VCALENDAR" + Crlf;

        /// <summary>
        ///   The default values for properties are defined when an instance is created.
        /// </summary>
        [TestMethod]
        public void Defaults()
        {
            var calendar = new VCalendar();
            Assert.AreEqual("Sepia.Calendaring", calendar.ProductId);
            Assert.AreEqual("2.0", calendar.Version);
            Assert.AreEqual("GREGORIAN", calendar.Scale);
        }

        [TestMethod]
        public void Reading()
        {
            var calendar = new VCalendar();
            calendar.ReadIcs(IcsReader.Create(new StringReader(Simple)));
            Assert.AreEqual("Zimbra-Calendar-Provider", calendar.ProductId);
            Assert.AreEqual("2.0", calendar.Version);
            Assert.AreEqual("GREGORIAN", calendar.Scale);
            Assert.AreEqual("REQUEST", calendar.Method);
        }

        [TestMethod]
        public void ReadingAllProperties()
        {
            string ics =
                "BEGIN:VCALENDAR" + Crlf +
                "CALSCALE:Coptic" + Crlf +
                "METHOD:REQUEST" + Crlf +
                "PRODID:sepia" + Crlf +
                "VERSION:2.0" + Crlf +
                "END:VCALENDAR" + Crlf;

            var calendar = new VCalendar();
            calendar.ReadIcs(IcsReader.Create(new StringReader(ics)));
            Assert.AreEqual("REQUEST", calendar.Method);
            Assert.AreEqual("sepia", calendar.ProductId);
            Assert.AreEqual("Coptic", calendar.Scale);
            Assert.AreEqual("2.0", calendar.Version);
        }

        [TestMethod]
        public void WritingAllProperties()
        {
            string ics =
                "BEGIN:VCALENDAR" + Crlf +
                "CALSCALE:Coptic" + Crlf +
                "METHOD:REQUEST" + Crlf +
                "PRODID:sepia" + Crlf +
                "VERSION:2.0" + Crlf +
                "END:VCALENDAR" + Crlf;

            var calendar = new VCalendar();
            calendar.ReadIcs(IcsReader.Create(new StringReader(ics)));
            var ics2 = new StringWriter();
            calendar.WriteIcs(IcsWriter.Create(ics2));
            Assert.AreEqual(ics, ics2.ToString());
        }

        [TestMethod]
        public void WritingComponents()
        {
            var calendar = new VCalendar()
            {
                Components = { new VTimeZone(), new VEvent() }
            };
            var ics = new StringWriter();
            calendar.WriteIcs(IcsWriter.Create(ics));
            StringAssert.Contains(ics.ToString(), "BEGIN:VTIMEZONE");
            StringAssert.Contains(ics.ToString(), "BEGIN:VEVENT");
        }

        [TestMethod]
        public void PropertiesAreCaseInsensitive()
        {
            string ics =
                "begin:vcalendar" + Crlf +
                "calscale:coptic" + Crlf +
                "method:request" + Crlf +
                "prodid:sepia" + Crlf +
                "version:2.0" + Crlf +
                "end:vcalendar" + Crlf;

            var calendar = new VCalendar();
            calendar.ReadIcs(IcsReader.Create(new StringReader(ics)));
            Assert.AreEqual("request", calendar.Method);
            Assert.AreEqual("sepia", calendar.ProductId);
            Assert.AreEqual("coptic", calendar.Scale);
            Assert.AreEqual("2.0", calendar.Version);
        }

        /// <summary>
        ///   Its easy to create a vCalendar for the local time zone.
        /// </summary>
        [TestMethod]
        public void Localised()
        {
            var calendar = VCalendar.ForLocalTimeZone();
            Console.WriteLine(calendar.ToString());
            Assert.IsTrue(calendar.Components.OfType<VTimeZone>().Any());
            Assert.IsTrue(calendar.Components.OfType<VTimeZone>().First().Adjustments.OfType<StandardChange>().Any());
        }
    }
}
