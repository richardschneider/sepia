﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia
{
    /// <summary>
    ///   Unit tests for <see cref="Tag{T}"/>.
    /// </summary>
    [TestClass]
    public class TypedTagTest
    {
        class Gender : Tag<Gender>
        {
            public static readonly Gender Male = new Gender { Authority = "Me", Name = "M" };
            public static readonly Gender Female = new Gender { Authority = "Me", Name = "F" };
        }

        /// <summary>
        ///   All tags have a URI, even when not well formed.
        /// </summary>
        [TestMethod]
        public void Uri()
        {
            var tag = new Gender { Authority = "sepia", Name = "x-test" };
            Assert.AreEqual("urn:sepia:x-test", tag.Uri);

            tag = new Gender();
            Assert.AreEqual("urn::", tag.Uri);
        }

        /// <summary>
        ///   Descriptions are not required and the list is never null.
        /// </summary>
        [TestMethod]
        public void NoDescriptions()
        {
            var tag = new Gender();
            Assert.IsNotNull(tag.Description);
            Assert.AreEqual(0, tag.Description.Count);
        }

        /// <summary>
        ///  Equality and hashing is based on the tag's URI which is case insensitive.
        /// </summary>
        [TestMethod]
        public void UriEqualityAndHashing()
        {
            var a = new Gender { Authority = "sepia", Name = "a" };
            var b = new Gender { Authority = "sepia", Name = "a" };
            var c = new Gender { Authority = "sepia", Name = "c" };
            var d = new Gender { Authority = "sepia", Name = "A" };

            Assert.AreEqual(a, b);
            Assert.AreNotEqual(a, c);
            Assert.AreEqual(a, d);

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
            Assert.AreEqual(a.GetHashCode(), d.GetHashCode());
        }

        /// <summary>
        ///   Strongly typed tag can be compared against untyped tags. 
        /// </summary>
        [TestMethod]
        public void TagsAreTags()
        {
            Assert.AreEqual(Tag.Unknown, Gender.Unknown);
            Assert.AreEqual(Gender.Unknown, Tag.Unknown);
            Assert.AreNotEqual(Tag.Unknown, Gender.Male);
            Assert.AreNotEqual(Gender.Male, Tag.Unknown);

            Assert.IsTrue(Gender.Unknown == Tag.Unknown);
            Assert.IsFalse(Gender.Female == Tag.Unknown);
            Assert.IsTrue(Tag.Unknown == Gender.Unknown);
            Assert.IsFalse(Tag.Unknown == Gender.Female);

            Assert.IsTrue(Gender.Unknown.Equals(Tag.Unknown));
            Assert.IsTrue(Tag.Unknown.Equals(Gender.Unknown));
            Assert.IsFalse(Gender.Male.Equals(Tag.Unknown));
            Assert.IsFalse(Tag.Unknown.Equals(Gender.Male));
        }
    }
}
