using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using CodePlex.XPathParser;
using System.Diagnostics;
using System.Xml.XPath;

namespace Sepia.Schematron.Queries
{
    /// <summary>
    ///   Rewrites an xpath 2 expression into xpath 1.
    /// </summary>
    public class XPath2Rewriter : IXPathBuilder<string> 
    {
        static string[] opStrings = { 
            /* Unknown    */ " Unknown ",
            /* Or         */ " or " ,
            /* And        */ " and ",
            /* Eq         */ "="    ,
            /* Ne         */ "!="   ,
            /* Lt         */ "<"    ,
            /* Le         */ "<="   ,
            /* Gt         */ ">"    ,
            /* Ge         */ ">="   ,
            /* Plus       */ "+"    ,
            /* Minus      */ "-"    ,
            /* Multiply   */ "*"    ,
            /* Divide     */ " div ",
            /* Modulo     */ " mod ",
            /* UnaryMinus */ "-"    ,
            /* Union      */ "|"  
        };

        static string[] axisStrings = {
            /*Unknown          */ "Unknown::"           ,
            /*Ancestor         */ "ancestor::"          ,
            /*AncestorOrSelf   */ "ancestor-or-self::"  ,
            /*Attribute        */ "attribute::"         ,
            /*Child            */ "child::"             ,
            /*Descendant       */ "descendant::"        ,
            /*DescendantOrSelf */ "descendant-or-self::",
            /*Following        */ "following::"         ,
            /*FollowingSibling */ "following-sibling::" ,
            /*Namespace        */ "namespace::"         ,
            /*Parent           */ "parent::"            ,
            /*Preceding        */ "preceding::"         ,
            /*PrecedingSibling */ "preceding-sibling::" ,
            /*Self             */ "self::"              ,
            /*Root             */ "root::"              ,
        };

        /// <summary>
        /// 
        /// </summary>
        public bool RewriteRequired { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public void StartBuild() 
        {
            RewriteRequired = false;
        }

        /// <summary>
        /// 
        /// </summary>
        public string EndBuild(string result)
        {
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public string String(string value)
        {
            return "'" + value + "'";
        }

        /// <summary>
        /// 
        /// </summary>
        public string Number(string value)
        {
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Operator(XPathOperator op, string left, string right)
        {
            // TODO: Error parsing exists(@a | @b)
            if (op == XPathOperator.Union && left == null)
                return right;

            if (op == XPathOperator.UnaryMinus) {
                return "-" + left;
            }
            
            // Need to change comparison operator because XPath 1 only deals with numbers.
            // If either operand is a number, then rewrite is not required.
            double d;
            if (double.TryParse(left, NumberStyles.AllowLeadingSign|NumberStyles.AllowDecimalPoint|NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out d)
             || double.TryParse(right, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowTrailingWhite, NumberFormatInfo.InvariantInfo, out d))
            {
                return left + opStrings[(int)op] + right;
            }
            switch (op)
            {
                case XPathOperator.Lt:
                    RewriteRequired = true;
                    return string.Format("rewrite-compare({0}, {1}) < 0", left, right);
                case XPathOperator.Le:
                    RewriteRequired = true;
                    return string.Format("rewrite-compare({0}, {1}) <= 0", left, right);
                case XPathOperator.Gt:
                    RewriteRequired = true;
                    return string.Format("rewrite-compare({0}, {1}) > 0", left, right);
                case XPathOperator.Ge:
                    RewriteRequired = true;
                    return string.Format("rewrite-compare({0}, {1}) >= 0", left, right);
                default:
                    return left + opStrings[(int)op] + right;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Axis(XPathAxis xpathAxis, XPathNodeType nodeType, string prefix, string name)
        {
            if (xpathAxis == XPathAxis.Root && nodeType == XPathNodeType.Element)
                return "/";
            string nodeTest;
            switch (nodeType) {
            case XPathNodeType.ProcessingInstruction:
                Debug.Assert(prefix == "");
                nodeTest = "processing-instruction(" + name + ")";
                break;
            case XPathNodeType.Text:
                Debug.Assert(prefix == null && name == null);
                nodeTest = "text()";
                break;
            case XPathNodeType.Comment:
                Debug.Assert(prefix == null && name == null);
                nodeTest = "comment()";
                break;
            case XPathNodeType.All:
                nodeTest = "node()";
                break;
            case XPathNodeType.Attribute:
            case XPathNodeType.Element:
            case XPathNodeType.Namespace:
                nodeTest = QNameOrWildcard(prefix, name);
                break;
            default:
                throw new ArgumentException("unexpected XPathNodeType", "XPathNodeType");
            }
            return axisStrings[(int)xpathAxis] + nodeTest;
        }

        /// <summary>
        /// 
        /// </summary>
        public string JoinStep(string left, string right)
        {
            if (left.EndsWith("/"))
                return left + right;

            return left + '/' + right;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Predicate(string node, string condition, bool reverseStep)
        {
            // TODO: Doesn't work "/foo[...]"    
            //if (!reverseStep) {
                // In this method we don't know how axis was represented in original XPath and the only 
                // difference between ancestor::*[2] and (ancestor::*)[2] is the reverseStep parameter.
                // to not store the axis from previous builder events we simply wrap node in the () here.
                // node = '(' + node + ')';
            //}
            return node + '[' + condition + ']';
        }

        /// <summary>
        /// 
        /// </summary>
        public string Variable(string prefix, string name)
        {
            return '$' + QName(prefix, name);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Function(string prefix, string name, IList<string> args)
        {
            string result = QName(prefix, name) + '(';
            for (int i = 0; i < args.Count; i++) {
                if (i != 0) {
                    result += ',';
                }
                result += args[i];
            }
            result += ')';
            return result;
        }

        private static string QName(string prefix, string localName) {
            if (prefix == null) {
                throw new ArgumentNullException("prefix");
            }
            if (localName == null) {
                throw new ArgumentNullException("localName");
            }
            return prefix == "" ? localName : prefix + ':' + localName;
        }

        private static string QNameOrWildcard(string prefix, string localName) {
            if (prefix == null) {
                Debug.Assert(localName == null);
                return "*";
            }
            if (localName == null) {
                Debug.Assert(prefix != "");
                return prefix + ":*";
            }
            return prefix == "" ? localName : prefix + ':' + localName;
        }


    }
}

