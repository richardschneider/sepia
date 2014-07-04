using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia
{
    /// <summary>
    ///   Unit tests for a <see cref="TagBag"/>.
    /// </summary>
    [TestClass]
    public class TagBagTest
    {
        /// <summary>
        ///   Tags can be indexed by the <see cref="ITag.Uri"/>
        /// </summary>
        [TestMethod]
        public void IndexedByUri()
        {
            var english = LanguageTag.English;
            var tags = new TagBag();
            tags.Add(english);
            Assert.AreEqual(english, tags[english.Uri]);
            Assert.AreEqual(english, tags[english.Uri.ToUpperInvariant()]);
            Assert.AreEqual(english, tags[english.Uri.ToLowerInvariant()]);
        }
    }
}
