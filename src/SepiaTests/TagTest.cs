using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia
{
    /// <summary>
    ///   Unit tests for <see cref="Tag"/>.
    /// </summary>
    [TestClass]
    public class TagTest
    {
        /// <summary>
        ///   All tags have a URI, even when not well formed.
        /// </summary>
        [TestMethod]
        public void Uri()
        {
            var tag = new Tag { Authority = "sepia", Name = "x-test" };
            Assert.AreEqual("urn:sepia:x-test", tag.Uri);

            tag = new Tag();
            Assert.AreEqual("urn::", tag.Uri);
        }

        /// <summary>
        ///   Descriptions are not required and the list is never null.
        /// </summary>
        [TestMethod]
        public void NoDescriptions()
        {
            var tag = new Tag();
            Assert.IsNotNull(tag.Description);
            Assert.AreEqual(0, tag.Description.Count);
        }

        /// <summary>
        ///  Equality and hashing is based on the tag's URI which is case insensitive.
        /// </summary>
        [TestMethod]
        public void UriEqualityAndHashing()
        {
            var a = new Tag { Authority = "sepia", Name = "a" };
            var b = new Tag { Authority = "sepia", Name = "a" };
            var c = new Tag { Authority = "sepia", Name = "c" };
            var d = new Tag { Authority = "sepia", Name = "A" };

            Assert.AreEqual(a, b);
            Assert.AreNotEqual(a, c);
            Assert.AreEqual(a, d);

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
            Assert.AreEqual(a.GetHashCode(), d.GetHashCode());
        }
    }
}
