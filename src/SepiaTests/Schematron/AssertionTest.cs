using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class AssertionTest : SchematronTest
   {
      [TestMethod]
      public void Reading()
      {
         SchematronDocument a = Load("Schematron/Samples/All.sch");
         Rule r = FindRule(a, "foo");

         Assertion assert = r.Assertions[0];
         Assert.AreEqual("diag-1", assert.Diagnostics);
         Assert.AreEqual("icon", assert.Icon);
         Assert.AreEqual("foo-rule", assert.ID);
         Assert.AreEqual("role", assert.Role);
         Assert.AreEqual("subject", assert.Subject);
         Assert.AreEqual(". = 'bar'", assert.Test);
         Assert.AreEqual("foo must be bar", assert.Message.ToString());
      }

      [TestMethod]
      public void ReadingISO()
      {
         SchematronDocument doc = new SchematronDocument();
         doc.Load("Schematron/Samples/AllISO.sch");
         Assertion a = doc.Patterns["testing"].Rules["foo"].Assertions[0];
         Assert.AreEqual("diag-1", a.Diagnostics);
         Assert.AreEqual("flag", a.Flag);
         Assert.AreEqual("fpi", a.Fpi);
         Assert.AreEqual("icon", a.Icon);
         Assert.AreEqual("foo-rule", a.ID);
         Assert.AreEqual("foo must be bar", a.Message.ToString());
         Assert.AreEqual("role", a.Role);
         Assert.AreEqual("see", a.See);
         Assert.AreEqual("subject", a.Subject);
         Assert.AreEqual(". = 'bar'", a.Test);
      }

   }
}
