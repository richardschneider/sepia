using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class RuleTest : SchematronTest
   {
      [TestMethod]
      public void Reading()
      {
         SchematronDocument a = Load("Samples/All.sch");
         Rule r = FindRule(a, "dummy");
         Assert.AreEqual("//dummy", r.Context);
         Assert.AreEqual("dummy", r.ID);
         Assert.AreEqual(false, r.IsAbstract);
         Assert.AreEqual("role", r.Role);
      }

      [TestMethod]
      public void ReadingISO()
      {
         SchematronDocument a = Load("Samples/AllISO.sch");
         Rule r = FindRule(a, "dummy");
         Assert.AreEqual("//dummy", r.Context);
         Assert.AreEqual("dummy", r.ID);
         Assert.AreEqual(false, r.IsAbstract);
         Assert.AreEqual("role", r.Role);
         Assert.AreEqual("flag", r.Flag);
         Assert.AreEqual("fpi", r.Fpi);
         Assert.AreEqual("icon", r.Icon);
         Assert.AreEqual("see", r.See);
         Assert.AreEqual("subject", r.Subject);
      }

      [TestMethod]
      public void ExtendsRule1()
      {
         SchematronValidator validator = new SchematronValidator(SchematronReader.ReadSchematron("Samples/All.sch"));
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<fixed-element name='x1' xmlns='urn:testing'>bad value</fixed-element>");
         ExceptionAssert.Throws<SchematronValidationException>(() => validator.Validate(doc), "/fixed-element must be 'fixed");
      }

      [TestMethod]
      public void ExtendsRule2()
      {
         SchematronValidator validator = new SchematronValidator(SchematronReader.ReadSchematron("Samples/All.sch"));
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<fixed-element name='X1' xmlns='urn:testing'>fixed</fixed-element>");
         ExceptionAssert.Throws<SchematronValidationException>(() => validator.Validate(doc), "The attribute 'name', of element /fixed-element, is required and must start with 'x'");
      }
   }
}
