using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia
{
    /// <summary>
    ///   Unit tests for <see cref="Text"/>.
    /// </summary>
    [TestClass]
    public class TextTest
    {
        /// <summary>
        ///   Supplied constructor values are honoured.
        /// </summary>
        [TestMethod]
        public void Construction()
        {
            var t = new Text("en", "hello world");
            Assert.AreEqual("en", t.Language);
            Assert.AreEqual("hello world", t.Value);
            Assert.AreEqual("hello world (en)", t.ToString());
        }

        /// <summary>
        ///   Text is value equality.
        /// </summary>
        [TestMethod]
        public void Equality()
        {
            var a = new Text("en", "hello world");
            var b = new Text("en", "hello world");
            var c = new Text("en-AU", "g'day mate");
            //Text d = null;

            Assert.IsTrue(a.Equals(a));
            Assert.IsTrue(a.Equals(b));
            Assert.IsFalse(a.Equals(c));
            //Assert.IsFalse(a.Equals(d));

            Assert.IsTrue(b.Equals(a));
            Assert.IsTrue(b.Equals(b));
            Assert.IsFalse(b.Equals(c));
            //Assert.IsFalse(b.Equals(d));

            Assert.IsFalse(c.Equals(a));
            Assert.IsFalse(c.Equals(b));
            Assert.IsTrue(c.Equals(c));
            //Assert.IsFalse(c.Equals(d));

            Assert.IsTrue(a.Equals((object) a));
            Assert.IsTrue(a.Equals((object) b));
            Assert.IsFalse(a.Equals((object) c));
            Assert.IsFalse(a.Equals((object) null));
        }

        /// <summary>
        ///   Nothing is equal to <b>null</b>.
        /// </summary>
        [TestMethod]
        public void NullEquality()
        {
            var a = new Text("en", "hello world");
            Assert.IsFalse(a  ==  null);
            Assert.IsTrue(a != null);
            Assert.IsFalse(a.Equals(null));
        }

        /// <summary>
        ///   <see cref="Text"/> has value equality.
        /// </summary>
        [TestMethod]
        public void EqualOperatator()
        {
            var a = new Text("en", "hello world");
            var b = new Text("en", "hello world");
            var c = new Text("en-AU", "g'day mate");

            Assert.IsTrue(a == a);
            Assert.IsTrue(a == b);
            Assert.IsFalse(a == c);

            Assert.IsTrue(b == a);
            Assert.IsTrue(b == b);
            Assert.IsFalse(b == c);

            Assert.IsFalse(c == a);
            Assert.IsFalse(c == b);
            Assert.IsTrue(c == c);
        }

        /// <summary>
        ///   <see cref="Text"/> has value equality.
        /// </summary>
        [TestMethod]
        public void NotEqualOperatator()
        {
            var a = new Text("en", "hello world");
            var b = new Text("en", "hello world");
            var c = new Text("en-AU", "g'day mate");

            Assert.IsFalse(a != a);
            Assert.IsFalse(a != b);
            Assert.IsTrue(a != c);

            Assert.IsFalse(b != a);
            Assert.IsFalse(b != b);
            Assert.IsTrue(b != c);

            Assert.IsTrue(c != a);
            Assert.IsTrue(c != b);
            Assert.IsFalse(c != c);
        }

        /// <summary>
        ///   Hash codes are the same for equal objects.
        /// </summary>
        [TestMethod]
        public void Hashing()
        {
            var a = new Text("en", "hello world");
            var b = new Text("en", "hello world");
            var c = new Text("en-AU", "g'day mate");

            Assert.AreEqual(a.GetHashCode(), b.GetHashCode());
            Assert.AreNotEqual(a.GetHashCode(), c.GetHashCode());
            ExceptionAssert.Throws<InvalidOperationException>(() => new Text().GetHashCode());
        }

        /// <summary>
        ///   A <see cref="Text"/> is implicitly cast to a <see cref="string"/>.
        /// </summary>
        [TestMethod]
        public void ImplictCastToString()
        {
            var t = new Text("en", "hello world");
            string s = t;

            Assert.AreEqual("hello world", s);
        }

#if false
        /// <summary>
        ///   A null <see cref="Text"/> is implicitly cast to an empty string.
        /// </summary>
        [TestMethod]
        public void NullImplictCastToString()
        {
            Text t = null;
            string s = t;

            Assert.AreEqual("", s);
        }
#endif

        /// <summary>
        ///   Once set a property cannot be changed.
        /// </summary>
        [TestMethod]
        public void ImmutableProperties()
        {
            var t = new Text();
            t.Language = "en";
            t.Value = "hello world";
            ExceptionAssert.Throws<InvalidOperationException>(() => t.Language = "en-NZ");
            ExceptionAssert.Throws<InvalidOperationException>(() => t.Value = "cheers");

            var t1 = new Text("en", "hello world");
            ExceptionAssert.Throws<InvalidOperationException>(() => t1.Language = "en-NZ");
            ExceptionAssert.Throws<InvalidOperationException>(() => t1.Value = "cheers");

            var t2 = new Text { Language = "en", Value = "hello world" };
            ExceptionAssert.Throws<InvalidOperationException>(() => t2.Language = "en-NZ");
            ExceptionAssert.Throws<InvalidOperationException>(() => t2.Value = "cheers");
        }

        /// <summary>
        ///   Language tags are case insensitive.
        /// </summary>
        [TestMethod]
        public void LanguageTagsAreCaseInsensitive()
        {
            var t1 = new Text("en", "hello world");
            var t2 = new Text("EN", "hello world");

            Assert.AreEqual(t1, t2);
            Assert.AreEqual(t1.GetHashCode(), t2.GetHashCode());
        }

    }
}
