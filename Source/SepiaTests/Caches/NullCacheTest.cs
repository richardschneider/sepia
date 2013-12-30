using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia.Caches
{
    /// <summary>
    ///   Units tests for <see cref="NullCache"/>
    /// </summary>
    [TestClass]
    public class NullCacheTest
    {
        /// <summary>
        ///   An URI can be resolved.
        /// </summary>
        [TestMethod]
        public void Resolving()
        {
            var cache = new NullCache();
            Assert.AreEqual("foo", cache.Resolve("urn:x-test:foo", (uri) => "foo"));
        }
    }
}
