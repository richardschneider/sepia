using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia
{
    /// <summary>
    ///   Unit tests for <see cref="Guard"/>.
    /// </summary>
    [TestClass]
    public class GuardTest
    {
        [TestMethod]
        public void IsNotNull()
        {
            object x = null;
            ExceptionAssert.Throws<ArgumentNullException>(() => Guard.IsNotNull(x, "x"));

            x = new object();
            Guard.IsNotNull(x, "x");
        }

        [TestMethod]
        public void IsMutable()
        {
            var p = new Popsicle();

            Guard.IsMutable(p.A, "A");
            Guard.IsMutable(p.B, "B");
            Guard.IsMutable(p.C, "C");

            p.A = 1;
            ExceptionAssert.Throws<InvalidOperationException>(() => Guard.IsMutable(p.A, "A"));
            Guard.IsMutable(p.B, "B");
            Guard.IsMutable(p.C, "C");

            p.B = p;
            ExceptionAssert.Throws<InvalidOperationException>(() => Guard.IsMutable(p.A, "A"));
            ExceptionAssert.Throws<InvalidOperationException>(() =>Guard.IsMutable(p.B, "B"));
            Guard.IsMutable(p.C, "C");

            p.C = new TimeSpan(10, 59, 30);
            ExceptionAssert.Throws<InvalidOperationException>(() => Guard.IsMutable(p.A, "A"));
            ExceptionAssert.Throws<InvalidOperationException>(() => Guard.IsMutable(p.B, "B"));
            ExceptionAssert.Throws<InvalidOperationException>(() => Guard.IsMutable(p.C, "C"));
        }

        private class Popsicle
        {
            public int A { get; set; }
            public Popsicle B { get; set; }
            public TimeSpan C { get; set; }
        }
 
        [TestMethod]
        public void Check()
        {
            Guard.Require(true, "foo", "bar");
            ExceptionAssert.Throws<ArgumentException>(() => Guard.Require(false, "foo", "bar"));
        }
    }
}
