using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class ActivePatternTest : SchematronTest
   {
      [TestMethod]
      public void Reading()
      {
         SchematronDocument doc = Load("Samples/All.sch");
         Assert.AreEqual("testing", doc.Phases["min"].ActivePatterns[0].Pattern);
         Assert.AreEqual("some text", doc.Phases["min"].ActivePatterns[0].Annotation.ToString());
      }


   }
}
