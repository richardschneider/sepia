using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class PhaseTest : SchematronTest
   {
      [TestMethod]
      public void Reading()
      {
         SchematronDocument a = Load("Samples/All.sch");

         Phase phase = a.Phases["min"];
         Assert.IsNotNull(phase);
         Assert.AreEqual(1, phase.ActivePatterns.Count);
         Assert.AreEqual("fpi", phase.Fpi);
         Assert.AreEqual("icon", phase.Icon);
         Assert.AreEqual("min", phase.ID);
         Assert.AreEqual("phase for minimal doc test. all for testing", phase.Annotation.ToString());

         phase = a.Phases["all"];
         Assert.IsNotNull(phase);
         Assert.AreEqual(3, phase.ActivePatterns.Count);
         Assert.AreEqual(String.Empty, phase.Fpi);
         Assert.AreEqual(String.Empty, phase.Icon);
         Assert.AreEqual("all", phase.ID);
         Assert.AreEqual("", phase.Annotation.ToString());
      }

      [TestMethod]
      public void ReadingISO()
      {
         SchematronDocument doc = Load("Samples/AllISO.sch");
         Phase phase = doc.Phases["all"];
         Assert.AreEqual(3, phase.ActivePatterns.Count);
         Assert.AreEqual("all", phase.Annotation.ToString());
         Assert.AreEqual("fpi", phase.Fpi);
         Assert.AreEqual(true, phase.HasParameters);
         Assert.AreEqual("icon", phase.Icon);
         Assert.AreEqual("all", phase.ID);
         Assert.AreEqual(2, phase.Parameters.Count);
         Assert.AreEqual("1", phase.Parameters["a"]);
         Assert.AreEqual("2", phase.Parameters["b"]);
         Assert.AreEqual("see", phase.See);
      }

   }
}
