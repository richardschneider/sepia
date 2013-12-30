using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia
{
    /// <summary>
    ///   Unit tests for <see cref="TextExtensions"/>.
    /// </summary>
    [TestClass]
    public class TextExtensionsTest
    {
        LanguageTag english = LanguageTag.Resolve("en");
        LanguageTag oz = LanguageTag.Resolve("en-AU");
        LanguageTag kiwi = LanguageTag.Resolve("en-NZ");

        [TestMethod]
        public void HasExactValue()
        {

            var greetings = new Text[]
            {
                new Text(english, "hello world"),
                new Text(oz, "g'day mate")
            };

            Assert.IsTrue(greetings.HasExactValue(english));
            Assert.IsTrue(greetings.HasExactValue(oz));
            Assert.IsFalse(greetings.HasExactValue(kiwi));
        }

        [TestMethod]
        public void WrittenIn()
        {
            var greetings = new Text[]
            {
                new Text(english, "hello world"),
                new Text(oz, "g'day mate")
            };

            Assert.AreEqual("hello world", greetings.WrittenIn(english));
            Assert.AreEqual("g'day mate", greetings.WrittenIn(oz));
            Assert.AreEqual("g'day mate", greetings.WrittenIn("en-AU-SYDNEY"));
            Assert.AreEqual("hello world", greetings.WrittenIn(kiwi));
            Assert.AreEqual("hello world", greetings.WrittenIn("fr"));
            Assert.AreEqual("hello world", greetings.WrittenIn("fr-CN"));

            Assert.AreEqual("", new Text[0].WrittenIn(english));
        }

        /// <summary>
        ///   Language tags are case insensitive.
        /// </summary>
        [TestMethod]
        public void  LanguageTagCaseInsensitive()
        {
            var greetings = new Text[]
            {
                new Text(english, "hello world"),
                new Text(oz, "g'day mate")
            };
            
            Assert.AreEqual("hello world", greetings.WrittenIn(english));
            Assert.AreEqual("g'day mate", greetings.WrittenIn(oz));
            Assert.AreEqual("g'day mate", greetings.WrittenIn("EN-au-SYDNEY"));
            Assert.AreEqual("hello world", greetings.WrittenIn(kiwi));
            Assert.AreEqual("hello world", greetings.WrittenIn("FR"));
            Assert.AreEqual("hello world", greetings.WrittenIn("FR-cn"));

            Assert.IsTrue(greetings.HasExactValue(english));
            Assert.IsTrue(greetings.HasExactValue(english));
            Assert.IsTrue(greetings.HasExactValue(english));
            Assert.IsTrue(greetings.HasExactValue(oz));
            Assert.IsTrue(greetings.HasExactValue(oz));
            Assert.IsTrue(greetings.HasExactValue(oz));
            Assert.IsTrue(greetings.HasExactValue(oz));
            Assert.IsTrue(greetings.HasExactValue(oz));
        }

    }
}
