using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia
{
    /// <summary>
    ///   Unit tests for <see cref="MultilingualText"/>
    /// </summary>
    [TestClass]
    public class MultilingualTextTest
    {
        LanguageTag english = LanguageTag.Resolve("en");
        LanguageTag oz = LanguageTag.Resolve("en-AU");
        LanguageTag kiwi = LanguageTag.Resolve("en-NZ");

        /// <summary>
        ///   Get indexing is sugar for <see cref="TextExtensions.WrittenIn"/>.
        /// </summary>
        [TestMethod]
        public void GetIndexSugar()
        {
            var greetings = new MultilingualText()
            {
                new Text(english, "hello world"),
                new Text(oz, "g'day mate")
            };

            Assert.AreEqual(greetings[english], greetings.WrittenIn(english));
            Assert.AreEqual(greetings[oz], greetings.WrittenIn(oz));
            Assert.AreEqual(greetings["en-au-sydney"], greetings.WrittenIn("en-AU-SYDNEY"));
            Assert.AreEqual(greetings[kiwi], greetings.WrittenIn(kiwi));
            Assert.AreEqual(greetings["FR"], greetings.WrittenIn("fr"));
            Assert.AreEqual(greetings["FR-cn"], greetings.WrittenIn("fr-CN"));

            var empty = new MultilingualText();
            Assert.AreEqual(empty[english], empty.WrittenIn(english));
        }
    }
}
