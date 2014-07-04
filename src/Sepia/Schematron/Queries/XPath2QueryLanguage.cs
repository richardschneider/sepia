using Common.Logging;
using Mvp.Xml.Exslt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using CodePlex.XPathParser;

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

        /// <summary>
        ///   Compile the XPath 2 expression.
        /// </summary>
        /// <remarks>
        ///   Rewrites expressions to conform to XPath 1.
        /// </remarks>
        protected override XPathExpression Compile(string xpath)
        {
            string xpath1;
            try
            {
                if (ToXpath1(xpath, out xpath1))
                {
                    if (log.IsDebugEnabled)
                        log.DebugFormat("rewrite '{0}' into '{1}'.", xpath, xpath1);

                    return base.Compile(xpath1);
                }
            }
            catch (Exception e)
            {
                log.ErrorFormat("Failed to rewrite '{0}' into xpath 1.", e, xpath);
            }

            return base.Compile(xpath);
        }

        bool ToXpath1(string xpath2, out string xpath1)
        {
            var builder = new XPath2Rewriter();
            xpath1 = new XPathParser<string>().Parse(xpath2, builder);

            return builder.RewriteRequired;
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
                { "compare", new StringCompareFunction() },
                { "numeric-compare", new NumericCompareFunction() },
            };

            public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
            {
                if (name == "rewrite-compare")
                {
                    name = argTypes.Any(a => a == XPathResultType.Number)
                        ? "numeric-compare"
                        : "compare";
                }
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

            class StringCompareFunction : IXsltContextFunction
            {
                public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
                {
                    var a = args[0] as string;
                    if (a == null)
                    {
                        var node = args[0] as XPathNodeIterator;
                        if (node != null && node.MoveNext())
                            a = node.Current.Value;
                    }

                    var b = args[1] as string;
                    if (b == null)
                    {
                        var node = args[1]as XPathNodeIterator;
                        if (node != null && node.MoveNext())
                            b = node.Current.Value;
                    }

                    // TODO: Culture/Collation aware
                    var result = StringComparer.InvariantCulture.Compare(a, b);
                    return result;
                }

                public XPathResultType[] ArgTypes { get { return new[] { XPathResultType.String, XPathResultType.String }; } }
                public int Maxargs { get { return 2; } }
                public int Minargs { get { return 2; } }
                public XPathResultType ReturnType { get { return XPathResultType.Number; } }
            }

            class NumericCompareFunction : IXsltContextFunction
            {
                public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
                {
                    var a = args[0] as double?;
                    if (a == null)
                    {
                        var node = args[0] as XPathNodeIterator;
                        if (node != null && node.MoveNext())
                            a = node.Current.ValueAsDouble;
                    }

                    var b = args[1] as double?;
                    if (b == null)
                    {
                        var node = args[1] as XPathNodeIterator;
                        if (node != null && node.MoveNext())
                            b = node.Current.ValueAsDouble;
                    }

                    return a.HasValue && b.HasValue
                        ? a.Value.CompareTo(b.Value)
                        : double.NaN;
                }

                public XPathResultType[] ArgTypes { get { return new[] { XPathResultType.String, XPathResultType.String }; } }
                public int Maxargs { get { return 2; } }
                public int Minargs { get { return 2; } }
                public XPathResultType ReturnType { get { return XPathResultType.Number; } }
            }
        
        }
    }
}
