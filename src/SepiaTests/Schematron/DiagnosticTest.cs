using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class DiagosticeTest : SchematronTest
   {
      [TestMethod]
      public void Reading()
      {
         SchematronDocument doc = Load("Schematron/Samples/All.sch");
         Assert.AreEqual("icon", doc.Diagnostics["diag-1"].Icon);
         Assert.AreEqual("diag-1", doc.Diagnostics["diag-1"].ID);
         Assert.AreEqual("diag # 1", doc.Diagnostics["diag-1"].Message.ToString());
      }

      [TestMethod]
      public void Annotation()
      {
         SchematronValidator validator = new SchematronValidator(SchematronReader.ReadSchematron("Schematron/Samples/All.sch"));
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<document><a><foobar>x-1</foobar><foobar>x-2</foobar></a></document>");
         errors.Clear();
         validator.Validate(doc, new SchematronValidationEventHandler(handler));
         Assert.AreEqual(2, errors.Count);
         Assert.AreEqual("'x-1' is not an allowed value.", errors[0].Diagnostics[0]);
         Assert.AreEqual("'x-2' is not an allowed value.", errors[1].Diagnostics[0]);
      }

      List<SchematronValidationEventArgs> errors = new List<SchematronValidationEventArgs>();
      void handler(object sender, SchematronValidationEventArgs e)
      {
         errors.Add(e);
      }
   }
}
