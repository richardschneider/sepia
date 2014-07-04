using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class ExsltTest : SchematronTest
   {


      [TestMethod]
      public void Math()
      {
         SchematronDocument schema = Load("Schematron/Samples/Exslt.sch");
         SchematronValidator validator = new SchematronValidator(schema);
         XmlDocument doc = new XmlDocument();

         doc.LoadXml("<prices><price>10</price><price>0</price></prices>");
         errors.Clear();
         validator.Validate(doc, new SchematronValidationEventHandler(handler));
         Assert.AreEqual(1, errors.Count);
         Assert.AreEqual("All prices must be greater than zero.", errors[0].Message);

         doc.LoadXml("<prices><price>10</price><price>2</price></prices>");
         errors.Clear();
         validator.Validate(doc, new SchematronValidationEventHandler(handler));
         Assert.AreEqual(0, errors.Count);
      }
      List<SchematronValidationEventArgs> errors = new List<SchematronValidationEventArgs>();
      void handler(object sender, SchematronValidationEventArgs e)
      {
         errors.Add(e);
      }

   }
}
