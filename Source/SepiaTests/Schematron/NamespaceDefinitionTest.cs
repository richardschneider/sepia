using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class NamespaceDefinitionTest : SchematronTest
   {
      [TestMethod]
      public void Reading()
      {
         SchematronDocument doc = Load("Samples/All.sch");
         Assert.AreEqual(1, doc.Namespaces.Count);
         Assert.AreEqual("t", doc.Namespaces[0].Prefix);
         Assert.AreEqual("urn:testing", doc.Namespaces[0].Uri);
      }


   }
}
