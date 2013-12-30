using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class SchematronReaderTest
   {
      [TestMethod]
      public void Reading()
      {
         SchematronDocument a = SchematronReader.ReadSchematron("Samples/UBL-ApplicationResponse-2.0.sch");
         Assert.IsNotNull(a);
      }

      [TestMethod]
      [ExpectedException(typeof(XmlSchemaValidationException))]
      public void ReadingBad()
      {
         SchematronReader.ReadSchematron("Samples/Bad1.sch");
      }
   }
}
