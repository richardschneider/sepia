using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class AnnotationTest : SchematronTest
   {

      [TestMethod]
      public void NameErrorMessage()
      {
         SchematronValidator validator = new SchematronValidator(SchematronReader.ReadSchematron("Schematron/Samples/All.sch"));
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<document xmlns='urn:x' xmlns:test='urn:testing'><test:a><test:dummy/></test:a></document>");
         ExceptionAssert.Throws<SchematronValidationException>(() => validator.Validate(doc), "The element /document/test:a/test:dummy is not allowed.");
      }

      [TestMethod]
      public void NameErrorMessage_NoPrefix()
      {
         SchematronValidator validator = new SchematronValidator(SchematronReader.ReadSchematron("Schematron/Samples/All.sch"));
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<dummy xmlns='urn:testing'/>");
         ExceptionAssert.Throws<SchematronValidationException>(() => validator.Validate(doc), "The element /dummy is not allowed.");
      }

   }
}
