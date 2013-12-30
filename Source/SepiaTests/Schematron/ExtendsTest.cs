using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class ExtendsTest : SchematronTest
   {
      [TestMethod]
      public void Reading()
      {
         SchematronDocument a = Load("Samples/All.sch");

         Rule r = FindRule(a, "rule-1");
         Assert.IsFalse(r.HasExtensions);

         r = FindRule(a, "rule-2");
         Assert.IsTrue(r.HasExtensions);
         Assert.AreEqual(1, r.Extends.Count);
         Assert.AreEqual("rule-1", r.Extends[0].RuleID);

         r = FindRule(a, "rule-3");
         Assert.IsTrue(r.HasExtensions);
         Assert.AreEqual(2, r.Extends.Count);
         Assert.AreEqual("rule-1", r.Extends[0].RuleID);
         Assert.AreEqual("rule-2", r.Extends[1].RuleID);

         r = FindRule(a, "rule-4");
         Assert.IsTrue(r.HasExtensions);
         Assert.AreEqual(1, r.Extends.Count);
         Assert.AreEqual("rule-2", r.Extends[0].RuleID);
      }

   }
}
