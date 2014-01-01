using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

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


    }

}
