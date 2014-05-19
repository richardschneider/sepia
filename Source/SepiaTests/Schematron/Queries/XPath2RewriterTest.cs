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
    public class XPath2RewriterTest
    {
        public virtual IQueryLanguage QueryLanguage
        {
            get { return new XPath2QueryLanguage(); }
        }

        /// <summary>
        ///   For a numeric compare, no rewrite is required.
        /// </summary>
        [TestMethod]
        public void NumericCompare()
        {
            var xpath2 = "foo[count(@a) > 0]";
            var builder = new XPath2Rewriter();
            var xpath1 = new XPathParser<string>().Parse(xpath2, builder);

            Console.WriteLine(xpath2);
            Console.WriteLine(xpath1);
            Assert.IsFalse(builder.RewriteRequired);
        }

        [TestMethod]
        public void Unions()
        {
            var xpath2 = "exists(@a | @b)";
            var builder = new XPath2Rewriter();
            var xpath1 = new XPathParser<string>().Parse(xpath2, builder);

            Console.WriteLine(xpath2);
            Console.WriteLine(xpath1);
            Assert.IsFalse(builder.RewriteRequired);
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
