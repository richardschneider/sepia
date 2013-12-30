using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class PatternTest : SchematronTest
   {
      [TestMethod]
      public void Reading()
      {
         SchematronDocument a = Load("Samples/All.sch");
         Pattern pattern = a.Patterns[0];

         Assert.AreEqual("icon", pattern.Icon);
         Assert.AreEqual("testing", pattern.ID);
         Assert.AreEqual("Testing", pattern.Title.ToString());
         Assert.AreEqual(2, pattern.Rules.Count);
         Assert.AreEqual("see", pattern.See);
         Assert.AreEqual(false, pattern.IsAbstract);
         Assert.AreEqual("", pattern.BasePatternID);
         Assert.AreEqual(0, pattern.Parameters.Count);

         Assert.AreEqual("A test pattern", a.Patterns["testing"].Annotation.ToString());
      }

      [TestMethod]
      public void ReadingISO()
      {
         SchematronDocument schema = Load("Samples/AbstractPattern.sch");

         Assert.IsTrue(schema.Patterns["requiredAttribute"].IsAbstract, "abstract");
         Assert.AreEqual("requiredAttribute", schema.Patterns["foo1"].BasePatternID);
         Assert.AreEqual("foo", schema.Patterns["foo1"].Parameters["context"]);
         Assert.AreEqual("@id", schema.Patterns["foo1"].Parameters["attribute"]);
         Assert.AreEqual("id is required", schema.Patterns["foo1"].Title.ToString());
      }

      [TestMethod]
      public void Abstract()
      {
         SchematronDocument schema = Load("Samples/AbstractPattern.sch");
         SchematronWriter writer = new SchematronWriter();
         writer.WriteDocument(schema.CompiledDocument, Console.Out);
         SchematronValidator validator = new SchematronValidator(schema);
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<doc><foo id='a' bar='x'/><foo id='b'/><foo bar='x'/><foo/></doc>");
         errors.Clear();
         validator.Validate(doc, new SchematronValidationEventHandler(handler));
         Assert.AreEqual(4, errors.Count);
         Assert.AreEqual("The /doc/foo[3] element is missing a required attribute.", errors[0].Message);
         Assert.AreEqual("The /doc/foo[4] element is missing a required attribute.", errors[1].Message);
         Assert.AreEqual("The /doc/foo[2] element is missing a required attribute.", errors[2].Message);
         Assert.AreEqual("The /doc/foo[4] element is missing a required attribute.", errors[3].Message);
      }

      List<SchematronValidationEventArgs> errors = new List<SchematronValidationEventArgs>();
      void handler(object sender, SchematronValidationEventArgs e)
      {
         errors.Add(e);
      }

   }
}
