using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class TitleTest : SchematronTest
   {
      [TestMethod]
      public void Loading()
      {
         SchematronDocument schematron = Load("Samples/UBL-ApplicationResponse-2.0.sch");
         Assert.AreEqual("Schema for UBL-ApplicationResponse-2.0; ; BRM", schematron.Title.ToString());
      }

      [TestMethod]
      public void Rtl()
      {
         SchematronDocument a = Load("Samples/Arabic.sch");
         Assert.AreEqual(3, a.Title.Count);
         Assert.AreEqual("The title is \"", a.Title[0].InnerText);
         Assert.AreEqual("<dir value=\"rtl\" xml:lang=\"ar\" xmlns=\"http://www.ascc.net/xml/schematron\">مفتاح معايير الويب!</dir>", ((XmlElement) a.Title[1]).OuterXml);
         Assert.AreEqual("\" in Arabic.", a.Title[2].InnerText);
      }
   }
}
