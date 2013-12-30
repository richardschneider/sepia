using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sepia.Calendaring.Serialization
{
    [TestClass]
    public class ComponentTest
    {
        /// <summary>
        ///   A <see cref="Component"/> can be created by its IANA name.
        /// </summary>
        [TestMethod]
        public void Create()
        {
            var component = Component.Create("VALARM");
            Assert.AreEqual(Component.Names.Alarm, component.Name);
        }

        /// <summary>
        ///   Experimental components can be created.
        /// </summary>
        [TestMethod]
        public void CreateExperimental()
        {
            var component = Component.Create("X-SomethingNew");
            Assert.AreEqual("X-SomethingNew", component.Name);
        }

    }
}
