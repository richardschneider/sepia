using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class ValidationReportTest
   {
      [TestMethod]
      public void IsSvrlSchematronCorrect()
      {
         Assert.IsNotNull(Schematron.Default.ValidationReportLanguage);
      }

      [TestMethod]
      public void DoggieReport()
      {
         XmlDocument dogs = new XmlDocument();
         dogs.LoadXml(@"
<dogs>
  <dog petname='spot'><nose/><ear/><bone/><ear/></dog>
  <dog petname='hungry'><nose/><ear/><ear/></dog>
  <dog petname='emanon'><nose/><ear/><bone/></dog>
  <dog petname='smelly'><ear/><bone/><ear/></dog>
</dogs>");
         StringBuilder svrl = new StringBuilder();
         SchematronValidator validator = new SchematronValidator("Samples/Dog.sch");
         ValidationReport report = new ValidationReport(validator, svrl);
         validator.Validate(dogs);
         Console.WriteLine(svrl.ToString());

         XmlDocument doggieReport = new XmlDocument();
         doggieReport.LoadXml(svrl.ToString());
         validator = new SchematronValidator(Schematron.Default.ValidationReportLanguage);
         validator.Validate(doggieReport);
      }

      [TestMethod]
      public void DoggieConsoleReport()
      {
         XmlDocument dogs = new XmlDocument();
         dogs.LoadXml(@"
<dogs>
  <dog petname='spot'><nose/><ear/><bone/><ear/></dog>
  <dog petname='hungry'><nose/><ear/><ear/></dog>
  <dog petname='emanon'><nose/><ear/><bone/></dog>
  <dog petname='smelly'><ear/><bone/><ear/></dog>
</dogs>");
         SchematronValidator validator = new SchematronValidator("Samples/Dog.sch");
         ValidationReport report = new ValidationReport(validator, Console.Out);
         validator.Validate(dogs);
      }

      [TestMethod]
      public void DoggieReportNoError()
      {
         XmlDocument dogs = new XmlDocument();
         dogs.LoadXml(@"
<dogs>
  <dog petname='spot'><nose/><ear/><bone/><ear/></dog>
</dogs>");
         StringBuilder svrl = new StringBuilder();
         SchematronValidator validator = new SchematronValidator("Samples/Dog.sch");
         ValidationReport report = new ValidationReport(validator, svrl);
         validator.Validate(dogs);
         Assert.IsFalse(report.HasValidationErrors);
      }

      [TestMethod]
      public void DoggieReportWithError()
      {
         XmlDocument dogs = new XmlDocument();
         dogs.LoadXml(@"
<dogs>
  <dog petname='spot'><nose/><ear/><bone/><ear/></dog>
  <dog petname='hungry'><nose/><ear/><ear/></dog>
  <dog petname='smelly'><ear/><bone/><ear/></dog>
</dogs>");
         StringBuilder svrl = new StringBuilder();
         SchematronValidator validator = new SchematronValidator("Samples/Dog.sch");
         ValidationReport report = new ValidationReport(validator, svrl);
         validator.Validate(dogs);
         Assert.IsTrue(report.HasValidationErrors);
      }
   }
}
