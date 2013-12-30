using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia.Model
{
    /// <summary>
    ///   Unit tests for <see cref="BusinessData"/>.
    /// </summary>
    [TestClass]
    public class BusinessDataTest
    {
        class Something : BusinessData { }

        /// <summary>
        ///   The id and type is included in the URI.
        /// </summary>
        [TestMethod]
        public void IdInUri()
        {
            var something = new Something { Id = "foo" };
            StringAssert.Contains(something.Uri, "foo");
            StringAssert.Contains(something.Uri, "something");
        }
    }
}
