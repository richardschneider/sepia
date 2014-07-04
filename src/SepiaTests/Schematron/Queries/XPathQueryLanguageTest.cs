using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Sepia.Schematron.Queries
{
   [TestClass]
   public class XPathQueryLanguageTest : Tests.SchematronTest
   {
      [TestMethod]
      public void Matching()
      {
         XPathQueryLanguage query = new XPathQueryLanguage();

         SchematronDocument a = Load("Schematron/Samples/All.sch");
         Rule rule = FindRule(a, "foo");

         XmlDocument doc;
         object matchContext;
         bool matches;

         doc = new XmlDocument();
         doc.LoadXml("<foo>bar</foo>");
         matchContext = query.CreateMatchContext(a, doc);
         XPathNavigator nav = doc.CreateNavigator();
         nav.MoveToFirstChild();
         matches = query.Match(rule, matchContext, nav);
         Assert.IsTrue(matches);

         doc = new XmlDocument();
         doc.LoadXml("<document><a></a></document>");
         matchContext = query.CreateMatchContext(a, doc);
         matches = query.Match(rule, matchContext, doc.CreateNavigator());
         Assert.IsFalse(matches);
      }

      [TestMethod]
      public void Asserting()
      {
         XPathQueryLanguage query = new XPathQueryLanguage();

         SchematronDocument a = Load("Schematron/Samples/All.sch");
         Rule rule = FindRule(a, "foo");
         Assertion assertion = (Assertion)rule.Assertions[0];

         XmlDocument doc;
         object matchContext;

         doc = new XmlDocument();
         doc.LoadXml("<doc><foo>bar</foo><foo>x</foo></doc>");
         matchContext = query.CreateMatchContext(a, doc);
         XPathNavigator nav = doc.DocumentElement.CreateNavigator();
         nav.MoveToFirstChild();
         Assert.IsTrue(query.Assert(assertion, matchContext, nav));

         nav.MoveToNext();
         Assert.IsFalse(query.Assert(assertion, matchContext, nav));
      }

      [TestMethod]
      public void Letting_Match()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<foo/>");
         XPathNavigator nav = doc.DocumentElement.CreateNavigator();

         XPathQueryLanguage query = new XPathQueryLanguage();
         Rule rule = new Rule();
         rule.Context = " $ename ";

         object context = query.CreateMatchContext(null, doc);
         query.Let(context, "ename", "foo");
         Assert.IsTrue(query.Match(rule, context, nav));

         context = query.CreateMatchContext(null, doc);
         query.Let(context, "ename", "bar");
         Assert.IsFalse(query.Match(rule, context, nav));
      }

      [TestMethod]
      public void Letting_Assert_String()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<foo>bar</foo>");
         XPathNavigator nav = doc.DocumentElement.CreateNavigator();

         XPathQueryLanguage query = new XPathQueryLanguage();
         object context = query.CreateMatchContext(null, doc);
         query.Let(context, "x", "'bar'");
         Assertion assert = new Assertion();
         assert.Test = ". = $x";
         Assert.IsTrue(query.Assert(assert, context, nav));

         context = query.CreateMatchContext(null, doc);
         query.Let(context, "x", "'foo'");
         Assert.IsFalse(query.Assert(assert, context, nav));
      }

      [TestMethod]
      public void Letting_Assert_Number()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<foo><a/><a/><a/></foo>");
         XPathNavigator nav = doc.DocumentElement.CreateNavigator();

         XPathQueryLanguage query = new XPathQueryLanguage();
         object context = query.CreateMatchContext(null, doc);
         query.Let(context, "x", "3");
         Assertion assert = new Assertion();
         assert.Test = "count(*) = $x";
         Assert.IsTrue(query.Assert(assert, context, nav));

         context = query.CreateMatchContext(null, doc);
         query.Let(context, "x", "2");
         Assert.IsFalse(query.Assert(assert, context, nav));
      }

      [TestMethod]
      public void Reporting()
      {
         XPathQueryLanguage query = new XPathQueryLanguage();

         SchematronDocument a = Load("Schematron/Samples/All.sch");
         Rule rule = FindRule(a, "dummy");
         Assertion assertion = (Assertion)rule.Assertions[0];

         XmlDocument doc;
         object matchContext;

         doc = new XmlDocument();
         doc.LoadXml("<dummy/>");
         matchContext = query.CreateMatchContext(a, doc);
         XPathNavigator nav = doc.CreateNavigator();
         nav.MoveToFirstChild();
         Assert.IsFalse(query.Assert(assertion, matchContext, nav));
      }

      [TestMethod]
      [ExpectedException(typeof(ArgumentException))]
      public void MultipleLets()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<foo><a/><a/><a/></foo>");
         XPathNavigator nav = doc.DocumentElement.CreateNavigator();

         XPathQueryLanguage query = new XPathQueryLanguage();
         object context = query.CreateMatchContext(null, doc);
         query.Let(context, "x", "3");
         query.Let(context, "x", "2");
      }

      [TestMethod]
      public void MultiScopedLets()
      {
         XmlDocument doc = new XmlDocument();
         doc.LoadXml("<foo>bar</foo>");
         XPathNavigator nav = doc.DocumentElement.CreateNavigator();
         Assertion assert = new Assertion();
         assert.Test = ". = $x";

         XPathQueryLanguage query = new XPathQueryLanguage();
         object context = query.CreateMatchContext(null, doc);

         query.Let(context, "x", "'bar'");
         Assert.IsTrue(query.Assert(assert, context, nav));

         query.PushScope(context);
         query.Let(context, "x", "'not-bar'");
         Assert.IsFalse(query.Assert(assert, context, nav));

         query.PopScope(context);
         Assert.IsTrue(query.Assert(assert, context, nav));
      }

   }

}
