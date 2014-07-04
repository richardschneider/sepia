using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class CompilerTest : SchematronTest
   {
      [TestMethod]
      public void Minimal()
      {
         SchematronDocument full = SchematronReader.ReadSchematron("Schematron/Samples/All.sch");
         SchematronDocument minimal = new Compiler().Compile(full);

         Assert.IsNotNull(minimal);
         Assert.AreEqual(full.DefaultPhase, minimal.DefaultPhase);
         Assert.AreEqual(full.Diagnostics.Count, minimal.Diagnostics.Count);
         Assert.AreEqual(full.ID, minimal.ID);
         Assert.AreEqual(full.Namespaces.Count, minimal.Namespaces.Count);
         Assert.AreEqual(full.Phases.Count, minimal.Phases.Count);
         Assert.AreEqual(full.QueryLanguage, minimal.QueryLanguage);
      }

      [TestMethod]
      public void RuleExtensions()
      {
         SchematronDocument full = SchematronReader.ReadSchematron("Schematron/Samples/All.sch");
         SchematronDocument minimal = new Compiler().Compile(full);

         Rule r = FindRule(minimal, "rule-4");
         Assert.AreEqual(3, r.Assertions.Count);
         Assert.AreEqual(". = 'fixed'", r.Assertions[0].Test);
         Assert.AreEqual("starts-with(@name, 'x')", r.Assertions[1].Test);
         Assert.AreEqual("1=1", r.Assertions[2].Test);
      }

      [TestMethod]
      public void MinimalIsValid()
      {
         SchematronDocument full = SchematronReader.ReadSchematron("Schematron/Samples/All.sch");
         SchematronDocument minimal = new Compiler().Compile(full);
         StringBuilder schematron = new StringBuilder();
         minimal.Save(schematron);

         XmlDocument minimalDocument = new XmlDocument();
         minimalDocument.LoadXml(schematron.ToString());

         SchematronValidator validator = new SchematronValidator(Schematron.Default.IsoSchematronSchema);
         validator.ValidationPhase = "minimal";
         validator.Validate(minimalDocument);

         SchematronDocument minimal2 = new SchematronDocument();
         minimal2.Load(schematron);
      }

      [TestMethod]
      public void CompilingAllSamples()
      {
          foreach (var x in Directory.EnumerateFiles("Schematron/Samples", "*.sch", SearchOption.AllDirectories))
          {
              Console.WriteLine(x);
              if (Path.GetFileName(x).StartsWith("Bad"))
                  continue;
              var doc = SchematronReader.ReadSchematron(x);
              new Compiler().Compile(doc);
          }
      }

   }
}
