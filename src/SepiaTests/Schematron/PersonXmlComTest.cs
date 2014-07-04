using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class PersonXmlCom : SchematronTest
   {
      [TestMethod]
      public void Valid()
      {
         SchematronValidator validator = new SchematronValidator("Schematron/Samples/Person.Xml.Com.sch");
         XmlDocument doc = new XmlDocument();
         doc.LoadXml(
@"<Person Title='Mr'>
    <Name>Eddie</Name>
    <Gender>Male</Gender>
</Person>");
         errors.Clear();
         validator.Validate(doc, new SchematronValidationEventHandler(handler));
         Assert.AreEqual(0, errors.Count);
      }

      [TestMethod]
      public void ElementOrder()
      {
         SchematronValidator validator = new SchematronValidator("Schematron/Samples/Person.Xml.Com.sch");
         XmlDocument doc = new XmlDocument();
         doc.LoadXml(
@"<Person Title='Mr'>
    <Gender>Male</Gender>
    <Name>Eddie</Name>
</Person>");
         ExceptionAssert.Throws<SchematronValidationException>(() => validator.Validate(doc), "The element Name must appear before element Gender.");
      }

      [TestMethod]
      public void GenderTitle()
      {
         SchematronValidator validator = new SchematronValidator("Schematron/Samples/Person.Xml.Com.sch");
         XmlDocument doc = new XmlDocument();
         doc.LoadXml(
@"<Person Title='Mr'>
    <Name>Eddie</Name>
    <Gender>Female</Gender>
</Person>");
         ExceptionAssert.Throws<SchematronValidationException>(() => validator.Validate(doc), "If the Title is \"Mr\" then the gender of the person must be \"Male\".");
      }

      [TestMethod]
      public void ElementMissing()
      {
         SchematronValidator validator = new SchematronValidator("Schematron/Samples/Person.Xml.Com.sch");
         XmlDocument doc = new XmlDocument();
         doc.LoadXml(
@"<Person Title='Mr'>
    <Name>Eddie</Name>
</Person>");
         ExceptionAssert.Throws<SchematronValidationException>(() => validator.Validate(doc), "The element Person should have the child elements Name and Gender.");
      }

      [TestMethod]
      public void TitleMissing()
      {
         SchematronValidator validator = new SchematronValidator("Schematron/Samples/Person.Xml.Com.sch");
         XmlDocument doc = new XmlDocument();
         doc.LoadXml(
@"<Person>
    <Gender>Male</Gender>
    <Name>Eddie</Name>
</Person>");
         ExceptionAssert.Throws<SchematronValidationException>(() => validator.Validate(doc), "The element Person must have a Title attribute");
      }

      List<SchematronValidationEventArgs> errors = new List<SchematronValidationEventArgs>();
      void handler(object sender, SchematronValidationEventArgs e)
      {
         errors.Add(e);
      }

   }
}
