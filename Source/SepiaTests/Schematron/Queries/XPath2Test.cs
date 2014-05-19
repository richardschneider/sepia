using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using CodePlex.XPathParser;

namespace Sepia.Schematron.Queries
{
    [TestClass]
    public class XPath2Test
    {
        public virtual IQueryLanguage QueryLanguage
        {
            get { return new XPath2QueryLanguage(); }
        }

        [TestMethod]
        public void Exists()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<foo><a/></foo>");
            XPathNavigator nav = doc.DocumentElement.CreateNavigator();
            var query = QueryLanguage;
            var context = query.CreateMatchContext(null, doc);

            var assert = new Assertion { Test = "exists(/foo)" };
            Assert.IsTrue(query.Assert(assert, context, nav), assert.Test);

            assert = new Assertion { Test = "exists(/foo/a)" };
            Assert.IsTrue(query.Assert(assert, context, nav), assert.Test);

            assert = new Assertion { Test = "exists(/foo/b, /foo/a, /foo)" };
            Assert.IsTrue(query.Assert(assert, context, nav), assert.Test);

            assert = new Assertion { Test = "exists(/bar)" };
            Assert.IsFalse(query.Assert(assert, context, nav), assert.Test);
        }

        [TestMethod]
        public void NumberCompare()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<foo a='3'/>");
            XPathNavigator nav = doc.DocumentElement.CreateNavigator();
            var query = QueryLanguage;
            var context = query.CreateMatchContext(null, doc);

            var assert = new Assertion { Test = "/foo[@a < 20]" };
            Assert.IsTrue(query.Assert(assert, context, nav), assert.Test, "a < 20");

            assert = new Assertion { Test = "/foo[@a < -1]" };
            Assert.IsFalse(query.Assert(assert, context, nav), assert.Test, "a < -1");

            assert = new Assertion { Test = "/foo[-1 < @a]" };
            Assert.IsTrue(query.Assert(assert, context, nav), assert.Test, "-1 < @a");
        }

        [TestMethod]
        public void StringCompare()
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<foo a='alpha' b='beta'/>");
            XPathNavigator nav = doc.DocumentElement.CreateNavigator();
            var query = QueryLanguage;
            var context = query.CreateMatchContext(null, doc);

            var assert = new Assertion { Test = "/foo[@a < @b]" };
            Assert.IsTrue(query.Assert(assert, context, nav), assert.Test, "a < b");
        }

        [TestMethod]
        public void ParsingErrors()
        {
            var big = "not(starts-with(f:reference/@value, '#')) or exists(ancestor::a:content/f:*/f:contained/f:*[@id=substring-after(current()/f:reference/@value, '#')]|/f:*/f:contained/f:*[@id=substring-after(current()/f:reference/@value, '#')])";
            var p1 = "not(starts-with(f:reference/@value, '#'))";
            var p2 = "ancestor::a:content/f:*/f:contained/f:*[@id=substring-after(current()/f:reference/@value, '#')]";
            var p3 = "/f:*/f:contained/f:*[@id=substring-after(current()/f:reference/@value, '#')]";
            var p4 = "/f:*[@id='#']";
            var p5 = "/f:foo[@id='#']";

            var xpath2 = p4;
            var builder = new XPath2Rewriter();
            var xpath1 = new XPathParser<string>().Parse(xpath2, builder);

            Console.WriteLine(xpath2);
            Console.WriteLine(xpath1);
        }
    }

}
