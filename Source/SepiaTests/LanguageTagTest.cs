using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia
{
    /// <summary>
    ///   Unit tests for <see cref="LanguageTag"/>.
    /// </summary>
    [TestClass]
    public class LanguageTagTest
    {
        /// <summary>
        ///   An IETF Language Subtag can be resolved to a <see cref="LanguageTag"/>.
        /// </summary>
        [TestMethod]
        public void Resolve()
        {
            var tag = LanguageTag.Resolve("fr");
            Assert.AreEqual("fr", tag.Name);
            StringAssert.StartsWith(tag.Authority, "ietf");
            Assert.IsTrue(tag.Description.Any(), "missing description for " + tag.ToString());
        }

        /// <summary>
        ///   Any language tag can be used, even unknown ones.
        /// </summary>
        [TestMethod]
        public void XUnknown()
        {
            var tag = LanguageTag.Resolve("x-unknown");
            Assert.AreEqual("x-unknown", tag.Name);
            Assert.AreEqual(0, tag.Description.Count);
        }

        /// <summary>
        ///   A language tag string can be used wherever a <see cref="LanguageTag"/> is needed.
        /// </summary>
        [TestMethod]
        public void ImplicitCastFromString()
        {
            var thai = new Text("th", "สวัสดี");
            var chineseSimplified = new Text("zh-Hans", "你好");
            var chineseTraditional = new Text("zh-Hant", "你好");

            Assert.AreEqual("Thai", thai.Language.Description.WrittenIn("en").Value);
            Assert.AreEqual("Chinese (Simplified)", chineseSimplified.Language.Description.WrittenIn("en").Value);
            Assert.AreEqual("Chinese (Traditional)", chineseTraditional.Language.Description.WrittenIn("en").Value);
        }
    }
}
