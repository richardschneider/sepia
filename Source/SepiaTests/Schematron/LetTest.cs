using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class LetTest : SchematronTest
   {
      [TestMethod]
      public void Reading()
      {
         SchematronDocument schema = Load("Samples/Let.sch");
         Assert.AreEqual("number(substring(.,1,2))", schema.Patterns[0].Rules[0].Parameters["hour"]);
         Assert.AreEqual("number(substring(.,4,2))", schema.Patterns[0].Rules[0].Parameters["minute"]);
         Assert.AreEqual("number(substring(.,7,2))", schema.Patterns[0].Rules[0].Parameters["second"]);
      }


      [TestMethod]
      public void Assertions() 
      {
         SchematronDocument schema = Load("Samples/Let.sch");
         SchematronValidator validator = new SchematronValidator(schema);
         XmlDocument doc = new XmlDocument();
         doc.LoadXml(
@"<times>
  <time>23:10:50</time>
  <time>11:10:50 PM</time>
  <time>24:10:50</time>
  <time>23:60:50</time>
  <time>23:10:60</time>
</times>");
         errors.Clear();
         validator.Validate(doc, new SchematronValidationEventHandler(handler));
         Assert.AreEqual(4, errors.Count);
         Assert.AreEqual("The time element should contain a time in the format HH:MM:SS.", errors[0].Message);
         Assert.AreEqual("The hour (24) be a value between 0 and 23.", errors[1].Message);
         Assert.AreEqual("The minutes (60) must be a value between 0 and 59.", errors[2].Message);
         Assert.AreEqual("The second (60) must be a value between 0 and 59.", errors[3].Message);
      }

      [TestMethod]
      public void PhaseAssertions()
      {
         SchematronDocument schema = Load("Samples/Let.sch");
         SchematronValidator validator = new SchematronValidator(schema);
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<times><time>23:10:00</time><time>23:10:50</time></times>");
         errors.Clear();
         validator.ValidationPhase = "no-seconds";
         validator.Validate(doc, new SchematronValidationEventHandler(handler));
         Assert.AreEqual(1, errors.Count);
         Assert.AreEqual("The second (50) must be a value between 0 and 0.", errors[0].Message);
      }

      [TestMethod]
      public void OverrideDocumentLet()
      {
         SchematronDocument schema = Load("Samples/Let.sch");
         SchematronValidator validator = new SchematronValidator(schema);
         validator.Parameters.Add("maxSec", "10");
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<times><time>23:10:00</time><time>23:10:50</time></times>");
         errors.Clear();
         validator.Validate(doc, new SchematronValidationEventHandler(handler));
         Assert.AreEqual(1, errors.Count);
         Assert.AreEqual("The second (50) must be a value between 0 and 10.", errors[0].Message);
      }

      List<SchematronValidationEventArgs> errors = new List<SchematronValidationEventArgs>();
      void handler(object sender, SchematronValidationEventArgs e)
      {
         errors.Add(e);
      }

   }
}
