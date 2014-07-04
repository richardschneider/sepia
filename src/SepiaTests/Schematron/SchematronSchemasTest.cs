using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Schema;
namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class SchematronSchemasTest
   {
      [TestMethod]
      public void Compiling()
      {
         XmlSchemaSet schemas = Schematron.Default.XsdSet;
         Assert.IsNotNull(schemas);
         Assert.AreNotEqual(0, schemas.Count);
         schemas.Compile();
      }
   }
}
