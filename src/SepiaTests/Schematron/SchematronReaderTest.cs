using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
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
         SchematronDocument a = SchematronReader.ReadSchematron("Schematron/Samples/UBL-ApplicationResponse-2.0.sch");
         Assert.IsNotNull(a);
      }

      [TestMethod]
      [ExpectedException(typeof(XmlSchemaValidationException))]
      public void ReadingBad()
      {
         SchematronReader.ReadSchematron("Schematron/Samples/Bad1.sch");
      }

       [TestMethod]
       public void ReadingAllSamples()
       {
           foreach (var x in Directory.EnumerateFiles("Schematron/Samples", "*.sch", SearchOption.AllDirectories))
           {
               Console.WriteLine(x);
               if (Path.GetFileName(x).StartsWith("Bad"))
               {
                   ExceptionAssert.Throws<XmlSchemaValidationException>(() => SchematronReader.ReadSchematron(x));
               }
               else
               {
                   SchematronReader.ReadSchematron(x);
               }
           }
       }
   }
}
