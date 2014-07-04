using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class SchematronDocumentTest : SchematronTest
   {
      [TestMethod]
      public void Reading()
      {
         SchematronDocument doc = Load("Schematron/Samples/All.sch");
         Assert.AreEqual("all", doc.DefaultPhase);
         Assert.AreEqual(2, doc.Diagnostics.Count);
         Assert.AreEqual("fpi", doc.Fpi);
         Assert.AreEqual("icon", doc.Icon);
         Assert.AreEqual("all-infoset", doc.ID);
         Assert.AreEqual(1, doc.Namespaces.Count);
         Assert.AreEqual(3, doc.Patterns.Count);
         Assert.AreEqual(2, doc.Phases.Count);
         Assert.AreEqual("xslt", doc.QueryLanguage);
         Assert.AreEqual("1.0", doc.SchemaVersion);
         Assert.IsNotNull(doc.Title);
         Assert.AreEqual("some text. some more text.", doc.Annotation.ToString());

         doc = Load("Schematron/Samples/AllISO.sch");
         Assert.AreEqual("all", doc.DefaultPhase);
         Assert.AreEqual(2, doc.Diagnostics.Count);
         Assert.AreEqual("fpi", doc.Fpi);
         Assert.AreEqual("icon", doc.Icon);
         Assert.AreEqual("all-infoset", doc.ID);
         Assert.AreEqual(1, doc.Namespaces.Count);
         Assert.AreEqual(3, doc.Patterns.Count);
         Assert.AreEqual(2, doc.Phases.Count);
         Assert.AreEqual("xpath", doc.QueryLanguage);
         Assert.AreEqual("1.0", doc.SchemaVersion);
         Assert.IsNotNull(doc.Title);
         Assert.AreEqual("some text. some more text.", doc.Annotation.ToString());
         Assert.AreEqual("see", doc.See);
      }

      [TestMethod]
      public void CompileAllQueryExpressions()
      {
          var doc = Load("Schematron/Samples/fhir-atom-modified.sch");
          doc.CompileQueryExpressions();
      }

   }
}
