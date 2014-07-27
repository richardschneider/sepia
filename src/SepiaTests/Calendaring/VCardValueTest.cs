using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Sepia.Calendaring.Serialization;

namespace Sepia.Calendaring
{

    /// <summary>
    ///   Unit tests for <see cref="VCardValue"/>.
    /// </summary>
    [TestClass]
    public class VCardValueTest
    {

        [TestMethod]
        public void Defaults()
        {
            var property = new VCardValue();
            Assert.IsNull(property.AlternativeId);
            Assert.IsFalse(property.Preference.HasValue);
            Assert.IsNull(property.Id);
        }

        [TestMethod]
        public void Parsing()
        {
            var property = new VCardValue(new ContentLine("EMAIL;type=home,work;PID=1.1,1.2;ALTID=3;PREF=2:jane_doe@example.com"));
            Assert.AreEqual("3", property.AlternativeId);
            Assert.AreEqual(2, property.Preference);
            Assert.AreEqual("1.1,1.2", property.Id);
            Assert.AreEqual("home,work", property.Type);
        }

        [TestMethod]
        public void WritingParameters()
        {
            var p0 = new VCardValue
            {
                AlternativeId = "2",
                Preference = 4,
                Id = "1.1",
                Type = "home",
            };
            var p1 = new VCardValue(p0.ToContentLine());
            Assert.AreEqual(p0.AlternativeId, p1.AlternativeId);
            Assert.AreEqual(p0.Preference, p1.Preference);
            Assert.AreEqual(p0.Id, p1.Id);
            Assert.AreEqual(p0.Type, p1.Type);
        }

    }
}
