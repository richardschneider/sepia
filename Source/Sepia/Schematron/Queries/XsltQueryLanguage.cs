using Common.Logging;
using Mvp.Xml.Exslt;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Sepia.Schematron.Queries
{
   /// <summary>
   ///   A Schematron Query Language that binds to "xslt".
   /// </summary>
   /// <remarks>
   ///   Provides access to the XSLT functions as define by W3C.
   /// </remarks>
   public class XsltQueryLanguage : XPathQueryLanguage
   {
      private static ILog log = LogManager.GetLogger(typeof(XPathQueryLanguage));

      internal override QueryContext CreateContext()
      {
         return new XsltContext1();
      }

      internal class XsltContext1 : XPathQueryLanguage.QueryContext
      {
         CurrentFunction currentFunction = new CurrentFunction();

         public override IXsltContextFunction ResolveFunction(string prefix, string name, XPathResultType[] argTypes)
         {
            if (string.IsNullOrEmpty(prefix))
            {
               if (name == "current" && argTypes.Length == 0)
                  return currentFunction;
            }

            return base.ResolveFunction(prefix, name, argTypes);
         }

         class CurrentFunction : IXsltContextFunction
         {
            #region IXsltContextFunction Members

            public XPathResultType[] ArgTypes
            {
               get { return null; }
            }

            public object Invoke(XsltContext xsltContext, object[] args, XPathNavigator docContext)
            {
               return ((QueryContext)xsltContext).current;
            }

            public int Maxargs
            {
               get { return 0; }
            }

            public int Minargs
            {
               get { return 0; }
            }

            public XPathResultType ReturnType
            {
               get { return XPathResultType.NodeSet; }
            }

            #endregion
         }

      }
   }
}
