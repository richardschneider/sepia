using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia.Calendaring.Serialization
{
    [TestClass]
    public class ContentLineTest
    {
        /// <summary>
        ///   The Parameters collection is lazily created.
        /// </summary>
        [TestMethod]
        public void ParametersAreLazy()
        {
            var line = new ContentLine();
            Assert.IsFalse(line.HasParameters);
            line.Parameters.Add("foo", "bar");
            Assert.IsTrue(line.HasParameters);
        }

        /// <summary>
        ///   Parameter names are case insensitive.
        /// </summary>
        [TestMethod]
        public void ParameterNameIsCaseInsensitive()
        {
            var line = new ContentLine("DESCRIPTION;Foo=1;BAR=2:This is a long description that exists on a long line.");
            Assert.AreEqual("DESCRIPTION", line.Name);
            Assert.AreEqual("This is a long description that exists on a long line.", line.Value);
            Assert.AreEqual("1", line.Parameters["foo"]);
            Assert.AreEqual("2", line.Parameters["bar"]);
        }

        /// <summary>
        ///   ToString() yields iCalendar content line.
        /// </summary>
        [TestMethod]
        public void Stringing()
        {
            var line = new ContentLine();
            line.Name = "DESCRIPTION";
            line.Parameters.Add("foo", "1");
            line.Parameters.Add("bar", "2");
            line.Parameters.Add("bar", "3");
            line.Value = "This is a long description\\, that exists on a long line.";
            Assert.AreEqual(@"DESCRIPTION;FOO=1;BAR=2,3:This is a long description\, that exists on a long line.", line.ToString());
        }

        /// <summary>
        ///   The value is used to convert content into a tag.
        /// </summary>
        [TestMethod]
        public void ToTag()
        {
            var content = new ContentLine("CLASS:PRIVATE");
            var tag = content.ToTag<AccessClassification>();
            Assert.AreEqual("PRIVATE", tag.Name);
            Assert.AreEqual("urn:ietf:rfc5545:PRIVATE", tag.Uri);
        }

        [TestMethod]
        public void ToTags()
        {
            var tags = new ContentLine("CATEGORIES:APPOINTMENT,EDUCATION").ToTags<Tag>().ToArray();
            Assert.AreEqual(2, tags.Length);
            Assert.AreEqual("APPOINTMENT", tags[0].Name);
            Assert.AreEqual("EDUCATION", tags[1].Name);
        }

        /// <summary>
        ///   The value is used to convert content into a date.
        /// </summary>
        [TestMethod]
        public void ToDate()
        {
            var date = new ContentLine("DTSTART:19970714T133000").ToDate();
            Assert.AreEqual(DateTimeKind.Local, date.Value.Kind);
            Assert.AreEqual(new DateTime(1997, 7, 14, 13, 30, 0, DateTimeKind.Local), date.Value);
            Assert.IsFalse(date.IsDateOnly);
            Assert.IsNull(date.TimeZone);

            date = new ContentLine("DTSTART;TZID=America/New_York:19970714T133000").ToDate();
            Assert.AreEqual(DateTimeKind.Local, date.Value.Kind);
            Assert.AreEqual(new DateTime(1997, 7, 14, 13, 30, 0, DateTimeKind.Local), date.Value);
            Assert.IsFalse(date.IsDateOnly);
            Assert.AreEqual("America/New_York", date.TimeZone);

            date = new ContentLine("DTSTART:19970714T173000Z").ToDate();
            Assert.AreEqual(DateTimeKind.Utc, date.Value.Kind);
            Assert.AreEqual(new DateTime(1997, 7, 14, 17, 30, 0, DateTimeKind.Utc), date.Value);
            Assert.IsFalse(date.IsDateOnly);
            Assert.IsNull(date.TimeZone);
        }

        /// <summary>
        ///   The value is used to convert content into a date.
        /// </summary>
        [TestMethod]
        public void ToText()
        {
            var text = new ContentLine("LOCATION;LANGUAGE=en:Germany").ToText();
            Assert.AreEqual("Germany", text.Value);
            Assert.AreEqual(LanguageTag.English, text.Language);

            text = new ContentLine("LOCATION;LANGUAGE=no:Tyskland").ToText();
            Assert.AreEqual("Tyskland", text.Value);
            Assert.AreEqual("no", text.Language.Name);
        }

        [TestMethod]
        public void ToTextNoLanguage()
        {
            var text = new ContentLine("LOCATION:Germany").ToText();
            Assert.AreEqual("Germany", text.Value);
            Assert.AreEqual(LanguageTag.Unspecified, text.Language);
        }

        [TestMethod]
        public void ToGeoCoordinate()
        {
            var geo = new ContentLine("GEO:37.386013;-122.082932").ToGeoCoordinate();
            Assert.AreEqual(37.386013d, geo.Latitude);
            Assert.AreEqual(-122.082932d, geo.Longitude);
        }

        [TestMethod]
        public void ToMailAddress()
        {
            var someone = new ContentLine("ORGANIZER;CN=John Smith:mailto:jsmith@example.com").ToMailAddress();
            Assert.AreEqual("John Smith", someone.DisplayName);
            Assert.AreEqual("jsmith@example.com", someone.Address);

            someone = new ContentLine("ORGANIZER:mailto:jsmith@example.com").ToMailAddress();
            Assert.AreEqual("", someone.DisplayName);
            Assert.AreEqual("jsmith@example.com", someone.Address);
        }

        [TestMethod]
        public void ToBoolean()
        {
            Assert.IsFalse(new ContentLine("X-Q:FALSE").ToBoolean());
            Assert.IsTrue(new ContentLine("X-Q:TRUE").ToBoolean());
            Assert.IsFalse(new ContentLine("X-Q:F").ToBoolean("t", "f"));
            Assert.IsTrue(new ContentLine("X-Q:T").ToBoolean("t", "f"));
            ExceptionAssert.Throws(() => new ContentLine("X-Q:X").ToBoolean());
        }

        [TestMethod]
        public void ToTimeZoneOffset()
        {
            Assert.AreEqual(new TimeSpan(4, 0, 0), new ContentLine("TZOFFSETFROM:+0400").ToTimeZoneOffset());
            Assert.AreEqual(new TimeSpan(4, 30, 0), new ContentLine("TZOFFSETFROM:+0430").ToTimeZoneOffset());
            Assert.AreEqual(new TimeSpan(4, 0, 0).Negate(), new ContentLine("TZOFFSETFROM:-0400").ToTimeZoneOffset());
            Assert.AreEqual(new TimeSpan(4, 30, 0).Negate(), new ContentLine("TZOFFSETFROM:-0430").ToTimeZoneOffset());

            Assert.AreEqual(new TimeSpan(4, 0, 0), new ContentLine("TZOFFSETFROM:+4").ToTimeZoneOffset());
            Assert.AreEqual(new TimeSpan(4, 0, 0), new ContentLine("TZOFFSETFROM:+04").ToTimeZoneOffset());
            Assert.AreEqual(new TimeSpan(4, 0, 0).Negate(), new ContentLine("TZOFFSETFROM:-4").ToTimeZoneOffset());
            Assert.AreEqual(new TimeSpan(4, 0, 0).Negate(), new ContentLine("TZOFFSETFROM:-04").ToTimeZoneOffset());
        }

        [TestMethod]
        public void ToRecurrenceDates()
        {
            Assert.AreEqual(
                new DateTime(1997, 07, 14, 12, 30, 00, DateTimeKind.Utc), 
                new ContentLine("RDATE:19970714T123000Z").ToRecurrenceDates().First().Value);
            Assert.AreEqual(
                new DateTime(1997, 07, 14, 12, 30, 00, DateTimeKind.Utc),
                new ContentLine("RDATE:19970714T123000Z,19980714T123000Z").ToRecurrenceDates().First().Value);
            Assert.AreEqual(
                new DateTime(1998, 07, 14, 12, 30, 00, DateTimeKind.Utc),
                new ContentLine("RDATE:19970714T123000Z,19980714T123000Z").ToRecurrenceDates().Last().Value);
        }

        [TestMethod]
        public void ToTimeSpan()
        {
            Assert.AreEqual(new TimeSpan(15, 5, 0, 20), new ContentLine("DURATION:P15DT5H0M20S").ToTimeSpan());
            Assert.AreEqual(new TimeSpan(5, 30, 0), new ContentLine("DURATION:PT5H30M").ToTimeSpan());
            Assert.AreEqual(new TimeSpan(0, 15, 0), new ContentLine("DURATION:PT15M").ToTimeSpan());
            Assert.AreEqual(new TimeSpan(1, 0, 0), new ContentLine("DURATION:PT1H0M0S").ToTimeSpan());
            Assert.AreEqual(new TimeSpan(1, 0, 0), new ContentLine("DURATION:+PT1H").ToTimeSpan());
            Assert.AreEqual(new TimeSpan(-1, 0, 0), new ContentLine("DURATION:-PT1H").ToTimeSpan());
            Assert.AreEqual(new TimeSpan(1, 0, 0).Negate(), new ContentLine("DURATION:-PT1H").ToTimeSpan());
            Assert.AreEqual(new TimeSpan(7 * 7, 0, 0, 0), new ContentLine("DURATION:P7W").ToTimeSpan());
            Assert.AreEqual(new TimeSpan(3, 5, 0, 0), new ContentLine("DURATION:P3DT5H").ToTimeSpan());
            Assert.AreEqual(new TimeSpan(7 * 7, 13, 0, 0), new ContentLine("DURATION:P7WT13H").ToTimeSpan());
            Assert.AreEqual(new TimeSpan(14 * 7, 0, 0, 0), new ContentLine("DURATION:P14W").ToTimeSpan());
            Assert.AreEqual(new TimeSpan(-14 * 7, 0, 0, 0), new ContentLine("DURATION:-P14W").ToTimeSpan());
            Assert.AreEqual(new TimeSpan(7 + 3, 5, 0, 0), new ContentLine("DURATION:P1W3DT5H").ToTimeSpan());
        }
    
    }
}
