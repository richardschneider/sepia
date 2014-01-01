using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace Sepia.Schematron.Queries
{
    /// <summary>
    ///   Includes all <see cref="XPath2Test"/> to guarantee that Xsl2 extends XPath2.
    /// </summary>
    [TestClass]
    public class Xslt2Test : XPath2Test
    {
        public override IQueryLanguage QueryLanguage
        {
            get { return new Xslt2QueryLanguage(); }
        }

    }

}
