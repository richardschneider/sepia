using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia.Caches
{
    /// <summary>
    ///   Units tests for <see cref="SimpleCache"/>
    /// </summary>
    [TestClass]
    public class SimpleCacheTest
    {
        /// <summary>
        ///   An URI can be resolved.
        /// </summary>
        [TestMethod]
        public void Resolving()
        {
            var cache = new SimpleCache();
            Assert.AreEqual("foo", cache.Resolve("urn:x-test:foo", (uri) => "foo"));
        }

        /// <summary>
        ///   Will return the same object when the object is in the cache.
        /// </summary>
        [TestMethod]
        public void SameObject()
        {
            var cache = new SimpleCache();
            var a = cache.Resolve("urn:x-test:foo1", (uri) => new object());
            var b = cache.Resolve("urn:x-test:foo1", (uri) => new object());
            var c = cache.Resolve("urn:x-test:foo1", (uri) => new object());
            Assert.AreSame(a, b);
            Assert.AreSame(a, c);
        }

        /// <summary>
        ///   After invalidating an item the cache returns a new object.
        /// </summary>
        [TestMethod]
        public void Invalidating()
        {
            const string id = "urn:x-test:invalidating";

            var cache = new SimpleCache();
            var a = cache.Resolve(id, (uri) => new object());
            cache.Invalidate(id);
            var b = cache.Resolve(id, (uri) => new object());
            var c = cache.Resolve(id, (uri) => new object());
            Assert.AreNotSame(a, b);
            Assert.AreSame(b, c);
        }

        /// <summary>
        ///   After evicting an item the cache returns a new object.
        /// </summary>
        [TestMethod]
        public void Evicting()
        {
            const string id = "urn:x-test:invalidating";

            var cache = new SimpleCache();
            var a = cache.Resolve(id, (uri) => new object());
            cache.Evict(id);
            var b = cache.Resolve(id, (uri) => new object());
            var c = cache.Resolve(id, (uri) => new object());
            Assert.AreNotSame(a, b);
            Assert.AreSame(b, c);
        }

        /// <summary>
        ///   URIs are case insensitive.
        /// </summary>
        [TestMethod]
        public void CaseInsenstive()
        {
            var cache = new SimpleCache();
            var a = cache.Resolve("urn:x-test:foo2", (uri) => new object());
            var b = cache.Resolve("urn:x-test:FOO2", (uri) => new object());
            var c = cache.Resolve("urn:x-test:Foo2", (uri) => new object());
            Assert.AreSame(a, b);
            Assert.AreSame(a, c);
        }

        /// <summary>
        ///   Circular dependencies are detected.
        /// </summary>
        [TestMethod]
        public void CircularDependency1()
        {
            var cache = new SimpleCache();

            Func<object> circular = () => cache.Resolve("urn:a", (a) => cache.Resolve("urn:b", (b) => cache.Resolve("urn:c", (c) => cache.Resolve("urn:a", (again) => new object()))));
            ExceptionAssert.Throws<CircularDependencyException>(() => circular());
        }


        /// <summary>
        ///   Circular dependencies are detected.
        /// </summary>
        [TestMethod]
        public void CircularDependency()
        {
            cacheUnderTest = new SimpleCache();

            ExceptionAssert.Throws<CircularDependencyException>(() => cacheUnderTest.Resolve("urn:a", NeedA));
        }

        private IObjectCache cacheUnderTest;

        object NeedA(string uri)
        {
            var b = cacheUnderTest.Resolve("urn:b", NeedB);
            return new object();
        }

        object NeedB(string uri)
        {
            var c = cacheUnderTest.Resolve("urn:c", NeedC);
            return new object();
        }

        object NeedC(string uri)
        {
            var a = cacheUnderTest.Resolve("urn:a", NeedA);
            return new object();
        }
    }
}
