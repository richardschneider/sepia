using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia
{
    /// <summary>
    ///   Unit tests for <see cref="TimeExtensions"/>.
    /// </summary>
    [TestClass]
    public class TimeExtensionsTest
    {
        [TestMethod]
        public void FuzzyEquality()
        {
            var now = DateTimeOffset.Now;
            var beyondDrift = TimeExtensions.DefaultDrift.Add(TimeSpan.FromSeconds(1));

            Assert.IsTrue(now.FuzzyEquals(now), "now ~= now");
            Assert.IsTrue(now.FuzzyEquals(now + TimeExtensions.DefaultDrift));
            Assert.IsTrue(now.FuzzyEquals(now - TimeExtensions.DefaultDrift));
            Assert.IsFalse(now.FuzzyEquals(now + beyondDrift));
            Assert.IsFalse(now.FuzzyEquals(now - beyondDrift));
        }

        [TestMethod]
        public void FuzzyComparison()
        {
            var now = DateTimeOffset.Now;
            var beyondDrift = TimeExtensions.DefaultDrift.Add(TimeSpan.FromSeconds(1));
            var drift = TimeExtensions.DefaultDrift;

            Assert.AreEqual(0, now.FuzzyCompare(now));
            Assert.AreEqual(0, now.FuzzyCompare(now + drift));
            Assert.AreEqual(0, now.FuzzyCompare(now - drift));
            Assert.IsTrue(now.FuzzyCompare(now + beyondDrift) < 0);
            Assert.IsTrue(now.FuzzyCompare(now - beyondDrift) > 0);
        }


        /// <summary>
        ///   Inclusive start and exclusive end times are used in fuzzy comparison.
        /// </summary>
        [TestMethod]
        public void IsIn()
        {
            var now = DateTimeOffset.Now;
            var start = now - TimeSpan.FromSeconds(10);
            var end = now + TimeSpan.FromSeconds(10);
            var tick = TimeSpan.FromTicks(1);

            Assert.IsTrue(now.Between(start, end));
            Assert.IsTrue(start.Between(start, end));
            Assert.IsFalse(end.Between(start, end));
            Assert.IsTrue((start + tick).Between(start, end));
            Assert.IsFalse((start - tick).Between(start, end));
            Assert.IsFalse((end + tick).Between(start, end));
            Assert.IsTrue((end - tick).Between(start, end));

            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => now.Between(end, start));

            DateTimeOffset? neverEnds = null;
            DateTimeOffset? start1 = start;
            Assert.IsTrue(now.Between(start, neverEnds));
            Assert.IsTrue(start.Between(start, neverEnds));
            Assert.IsTrue((start + tick).Between(start, neverEnds));
            Assert.IsFalse((start - tick).Between(start, neverEnds));
            Assert.IsTrue(DateTimeOffset.MaxValue.Between(start, neverEnds));

            ExceptionAssert.Throws<ArgumentOutOfRangeException>(() => now.Between(end, start1));
        }

    }
}
