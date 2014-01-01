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
    ///   A Schematron Query Language that binds to XPath2.
    /// </summary>
    /// <remarks>
    ///   Provides access to the XPath2 functions as define by W3C.
    ///   <note>Very minimal implementation.  Data types and most functions are not implemented.</note>
    /// </remarks>
    public class XPath2QueryLanguage : XPathQueryLanguage
    {
        static ILog log = LogManager.GetLogger(typeof(XPath2QueryLanguage));

        internal override QueryContext CreateContext()
        {
            return new XPath2Context();
        }

        internal class XPath2Context : XPathQueryLanguage.QueryContext
        {
            Dictionary<string, IXsltContextFunction> functions = new Dictionary<string, IXsltContextFunction>
            {
                { "exists", new ExistsFunction() },
                { "empty", new EmptyFunction() },
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

            class ExistsFunction : IXsltContextFunction
            {
                public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
                {
                    return args.OfType<XPathNodeIterator>()
                       .Any(node => node.Count > 0);
                }

                public XPathResultType[] ArgTypes { get { return new [] { XPathResultType.NodeSet }; } }
                public int Maxargs { get { return int.MaxValue; } }
                public int Minargs { get { return 1; } }
                public XPathResultType ReturnType { get { return XPathResultType.Boolean; } }
            }

            class EmptyFunction : IXsltContextFunction
            {
                public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
                {
                    return args.OfType<XPathNodeIterator>()
                       .All(node => node.Count == 0);
                }

                public XPathResultType[] ArgTypes { get { return new[] { XPathResultType.NodeSet }; } }
                public int Maxargs { get { return int.MaxValue; } }
                public int Minargs { get { return 1; } }
                public XPathResultType ReturnType { get { return XPathResultType.Boolean; } }
            }
        
        }
    }
}
