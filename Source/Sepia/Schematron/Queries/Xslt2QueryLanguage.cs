using Common.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Sepia.Schematron.Queries
{
    /// <summary>
    ///   A Schematron Query Language that binds to Xslt2.
    /// </summary>
    /// <remarks>
    ///   Provides access to the XSlt2 functions as define by W3C.
    ///   <note>Very minimal implementation.  Most functions are not implemented.</note>
    /// </remarks>
    public class Xslt2QueryLanguage : XPath2QueryLanguage
    {
        static ILog log = LogManager.GetLogger(typeof(Xslt2QueryLanguage));

        internal override QueryContext CreateContext()
        {
            return new Xslt2Context();
        }

        internal class Xslt2Context : XPath2QueryLanguage.XPath2Context
        {
            Dictionary<string, IXsltContextFunction> functions = new Dictionary<string, IXsltContextFunction>
            {
                { "current", new CurrentFunction() },
            };

            public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
            {
                if (log.IsDebugEnabled)
                    log.Debug(String.Format("Resolving function {0}:{1}", prefix, name));

                IXsltContextFunction function = null;
                if (string.IsNullOrEmpty(prefix))
                    functions.TryGetValue(name, out function);

                return function ?? base.ResolveFunction(prefix, name, argTypes);
            }

            class CurrentFunction : IXsltContextFunction
            {
                public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
                {
                    return ((QueryContext)xsltContext).current;
                }

                public XPathResultType[] ArgTypes { get { return null; } }
                public int Maxargs { get { return 0; } }
                public int Minargs { get { return 0; } }
                public XPathResultType ReturnType { get { return XPathResultType.NodeSet; } }
            }
        
        }
    }
}
