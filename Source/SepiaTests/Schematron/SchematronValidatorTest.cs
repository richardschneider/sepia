using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Sepia.Schematron
{
   [TestClass]
   public class SchematronValidatorTest : Tests.SchematronTest
   {
      //[TestMethod] 
      //public void WalkSpike()
      //{
      //   XmlDocument doc = new XmlDocument();
      //   doc.Load("Samples/All.sch");
      //   XPathNavigator nav = doc.CreateNavigator();
      //      Walk(nav);
      //}

      //void Walk(XPathNavigator nav)
      //{
      //   Console.WriteLine("{0} {1}", nav.NodeType, nav.Name);
      //   if (nav.HasChildren)
      //   {
      //      nav.MoveToFirstChild();
      //      do
      //      {
      //         Walk(nav);
      //      } while (nav.MoveToNext());
      //      nav.MoveToParent();
      //   }
      //}

      [TestMethod]
      public void SimpleValidating()
      {
         SchematronValidator validator = new SchematronValidator(SchematronReader.ReadSchematron("Samples/All.sch"));
         XmlDocument doc;

         doc = new XmlDocument();
         doc.LoadXml("<document><a><foo>bar</foo></a></document>");
         Assert.IsTrue(TryValidating(validator, doc));

         doc = new XmlDocument();
         doc.LoadXml("<document><a><foo>x</foo></a></document>");
         Assert.IsFalse(TryValidating(validator, doc));

         doc = new XmlDocument();
         doc.LoadXml("<document><a><foo>bar</foo><dummy/></a></document>");
         Assert.IsFalse(TryValidating(validator, doc));
      }

      [TestMethod]
      public void SimpleValidating2()
      {
         SchematronValidator validator = new SchematronValidator("Samples/All.sch");
         XmlDocument instance = new XmlDocument();
         instance.LoadXml("<document><a><foo>bar</foo></a></document>");
         validator.Validate(instance);
      }

      [TestMethod]
      public void SimpleErrorMessage()
      {
         SchematronValidator validator = new SchematronValidator(SchematronReader.ReadSchematron("Samples/All.sch"));
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<document><a><foo>x</foo></a></document>");
         ExceptionAssert.Throws<SchematronValidationException>(() => validator.Validate(doc), "foo must be bar");
      }

      [TestMethod]
      public void ComplexErrorMessage1()
      {
         SchematronValidator validator = new SchematronValidator(SchematronReader.ReadSchematron("Samples/All.sch"));
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<document><a><foobar>x</foobar></a></document>");
         ExceptionAssert.Throws<SchematronValidationException>(() => validator.Validate(doc), "The element '/document/a/foobar' must be foobar");
      }

      [TestMethod]
      public void ComplexErrorMessage2()
      {
         SchematronValidator validator = new SchematronValidator(SchematronReader.ReadSchematron("Samples/All.sch"));
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<document><a><foobar>foobar</foobar><foobar>x</foobar></a></document>");
         ExceptionAssert.Throws<SchematronValidationException>(() => validator.Validate(doc), "The element '/document/a/foobar[2]' must be foobar");
      }

      [TestMethod]
      public void MultipleErrors()
      {
         SchematronValidator validator = new SchematronValidator(SchematronReader.ReadSchematron("Samples/All.sch"));
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<document><a><foobar>x</foobar><foobar>x</foobar></a></document>");
         errors.Clear();
         validator.Validate(doc, new SchematronValidationEventHandler(handler));
         Assert.AreEqual(2, errors.Count);
         Assert.AreEqual("The element '/document/a/foobar[1]' must be foobar", errors[0].Message);
         Assert.AreEqual("The element '/document/a/foobar[2]' must be foobar", errors[1].Message);
      }

      List<SchematronValidationEventArgs> errors = new List<SchematronValidationEventArgs>();
      void handler(object sender, SchematronValidationEventArgs e)
      {
         errors.Add(e);
      }

      [TestMethod]
      public void PhasedValidating()
      {
         SchematronValidator validator = new SchematronValidator(SchematronReader.ReadSchematron("Samples/All.sch"));
         XmlDocument doc;
         doc = new XmlDocument();
         doc.LoadXml("<document><a><foobar>x</foobar></a></document>");
         Assert.IsFalse(TryValidating(validator, doc));
         validator.ValidationPhase = "min";
         Assert.IsTrue(TryValidating(validator, doc));
      }

      [TestMethod]
      public void BadPhaseName()
      {
         SchematronValidator validator = new SchematronValidator(SchematronReader.ReadSchematron("Samples/All.sch"));
         XmlDocument doc;
         doc = new XmlDocument();
         doc.LoadXml("<document><a><foobar>x</foobar></a></document>");
         validator.ValidationPhase = "undefined";
         ExceptionAssert.Throws<ArgumentException>(() => validator.Validate(doc), "'undefined' is not a defined phase.");
      }

      [TestMethod]
      public void ValidatingSchematronDocument()
      {
         SchematronValidator validator = new SchematronValidator(SchematronReader.ReadSchematron("Samples/Schematron-1.5.sch"));
         validator.ValidationPhase = Phase.All;
         XmlDocument doc = new XmlDocument();
         doc.Load("Samples/All.sch");
         Assert.IsTrue(TryValidating(validator, doc));
      }

      [TestMethod]
      public void PostalZone()
      {
         SchematronValidator validator = new SchematronValidator("Samples/PostalZone.sch");
         XmlDocument instance = new XmlDocument();
         instance.Load("Samples/PostalZone.ubl");
         StringBuilder svrl = new StringBuilder();
         ValidationReport report = new ValidationReport(validator, svrl);
         validator.Validate(instance);
         Console.WriteLine(svrl.ToString());
      }

   }
}
