using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class XPathHelperTest
   {
      [TestMethod]
      public void IndexesOneRelative()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<doc xmlns='urn:foo'><a/><a/><a/></doc>");
         XPathNavigator nav = doc.DocumentElement.CreateNavigator();
         Assert.AreEqual("/doc", XPathHelper.FullName(nav));

         nav.MoveToFirstChild();
         Assert.AreEqual("/doc/a[1]", XPathHelper.FullName(nav));

         nav.MoveToNext();
         Assert.AreEqual("/doc/a[2]", XPathHelper.FullName(nav));
      
         nav.MoveToNext();
         Assert.AreEqual("/doc/a[3]", XPathHelper.FullName(nav));
      }
   }
}
