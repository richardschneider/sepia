using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sepia.Schematron.Tests
{
   [TestClass]
   public class ReportTest : SchematronTest
   {
      [TestMethod]
      public void Reading()
      {
         SchematronDocument a = Load("Samples/All.sch");
         Rule r = FindRule(a, "dummy");

         Report report = (Report) r.Assertions[0];
         Assert.AreEqual("diag-1", report.Diagnostics);
         Assert.AreEqual("icon", report.Icon);
         Assert.AreEqual("dummy-rule", report.ID);
         Assert.AreEqual("role", report.Role);
         Assert.AreEqual("subject", report.Subject);
         Assert.AreEqual(".", report.Test);
      }


   }
}
