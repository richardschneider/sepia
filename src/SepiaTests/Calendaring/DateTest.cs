using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia.Calendaring
{
    [TestClass]
    public class DateTest
    {
        [TestMethod]
        public void Parsing()
        {
            var date = Date.Parse("19570813");
            Assert.AreEqual(DateTimeKind.Unspecified, date.Value.Kind);
            Assert.AreEqual(new DateTime(1957, 8, 13), date.Value);
            Assert.IsTrue(date.IsDateOnly);
            Assert.IsNull(date.TimeZone);

            date = Date.Parse("19970714T133000");
            Assert.AreEqual(DateTimeKind.Local, date.Value.Kind);
            Assert.AreEqual(new DateTime(1997, 7, 14, 13, 30, 0, DateTimeKind.Local), date.Value);
            Assert.IsFalse(date.IsDateOnly);
            Assert.IsNull(date.TimeZone);

            date = Date.Parse("19970714T133000", "America/New_York");
            Assert.AreEqual(DateTimeKind.Local, date.Value.Kind);
            Assert.AreEqual(new DateTime(1997, 7, 14, 13, 30, 0, DateTimeKind.Local), date.Value);
            Assert.IsFalse(date.IsDateOnly);
            Assert.AreEqual("America/New_York", date.TimeZone);

            date = Date.Parse("19970714T173000Z");
            Assert.AreEqual(DateTimeKind.Utc, date.Value.Kind);
            Assert.AreEqual(new DateTime(1997, 7, 14, 17, 30, 0, DateTimeKind.Utc), date.Value);
            Assert.IsFalse(date.IsDateOnly);
            Assert.IsNull(date.TimeZone);
        }

        [TestMethod]
        public void Stringing()
        {
            var date = new Date(new DateTime(1957, 8, 13), null, true);
            Assert.AreEqual("19570813", date.ToString());

            date = new Date(new DateTime(1997, 7, 14, 13, 30, 0, DateTimeKind.Local), null);
            Assert.AreEqual("19970714T133000 (local)", date.ToString());

            date = new Date(new DateTime(1997, 7, 14, 13, 30, 0, DateTimeKind.Local), "America/New_York");
            Assert.AreEqual("19970714T133000 (America/New_York)", date.ToString());

            date = new Date(new DateTime(1997, 7, 14, 17, 30, 0, DateTimeKind.Utc), null);
            Assert.AreEqual("19970714T173000Z", date.ToString());
        }
    }
}
