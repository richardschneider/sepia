using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Makaretu.CalendarScheduling.Types
{
    [TestClass]
    public class ClassificationTest
    {
        /// <summary>
        ///   The default value is <see cref="Classification.Public"/>.
        /// </summary>
        [TestMethod]
        public void DefaultValue()
        {
            var component = new Event();
            Assert.AreEqual(Classification.Public, component.Classification);
        }

        /// <summary>
        ///   A value can specified be programmatically.
        /// </summary>
        [TestMethod]
        public void NonDefaultValue()
        {
            var component = new Event() { Classification = Classification.Private };
            Assert.AreEqual(Classification.Private, component.Classification);
        }

        /// <summary>
        ///   An experimental can be specified programmatically.
        /// </summary>
        [TestMethod]
        public void ExperimentalValue()
        {
            var component = new Event();
            component.Properties[PropertyName.Classification] = new ContentLine() { Value = "X-SemiPrivate" };
            Assert.AreEqual("X-SemiPrivate", component.Properties[PropertyName.Classification].Value);
        }

        /// <summary>
        ///   An unknown value throws an <see cref="ArgumentException"/> when accessed.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void UnknownValue()
        {
            var component = new Event();
            component.Properties[PropertyName.Classification] = new ContentLine() { Value = "SemiPrivate" };
            var value = component.Classification;
        }

    }
}
