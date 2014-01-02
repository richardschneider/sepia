using Common.Logging;
using Mvp.Xml.Exslt;
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
            protected static ExsltSets ExsltSets = new ExsltSets();

            Dictionary<string, IXsltContextFunction> functions = new Dictionary<string, IXsltContextFunction>
            {
                { "distinct-values", new DistinctValuesFunction() },
                { "empty", new EmptyFunction() },
                { "exists", new ExistsFunction() },
                { "lower-case", new LowerCaseFunction() },
                { "upper-case", new UpperCaseFunction() },
            };

            public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
            {
                IXsltContextFunction function = null;
                if (string.IsNullOrEmpty(prefix))
                    functions.TryGetValue(name, out function);

                return function ?? base.ResolveFunction(prefix, name, argTypes);
            }

            class DistinctValuesFunction : IXsltContextFunction
            {
                public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
                {
                    return ExsltSets.distinct((XPathNodeIterator)args[0]);
                }

                public XPathResultType[] ArgTypes { get { return new[] { XPathResultType.NodeSet }; } }
                public int Maxargs { get { return 1; } }
                public int Minargs { get { return 1; } }
                public XPathResultType ReturnType { get { return XPathResultType.NodeSet; } }
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

            class LowerCaseFunction : IXsltContextFunction
            {
                public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
                {
                    return ((string)args[0]).ToLowerInvariant();
                }

                public XPathResultType[] ArgTypes { get { return new[] { XPathResultType.String }; } }
                public int Maxargs { get { return 1; } }
                public int Minargs { get { return 1; } }
                public XPathResultType ReturnType { get { return XPathResultType.String; } }
            }

            class UpperCaseFunction : IXsltContextFunction
            {
                public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
                {
                    return ((string)args[0]).ToUpperInvariant();
                }

                public XPathResultType[] ArgTypes { get { return new[] { XPathResultType.String }; } }
                public int Maxargs { get { return 1; } }
                public int Minargs { get { return 1; } }
                public XPathResultType ReturnType { get { return XPathResultType.String; } }
            }

        }
    }
}
