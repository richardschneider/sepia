using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace Sepia.Schematron
{
   /// <summary>
   ///   A collection of <see cref="XmlNode"/> objects.
   /// </summary>
   public class Annotation : List<XmlNode>
   {
      /// <summary>
      ///   Returns the <see cref="XmlNode.InnerText"/> of all nodes.
      /// </summary>
      public override string ToString()
      {
         StringBuilder s = new StringBuilder();
         foreach (XmlNode node in this)
         {
            s.Append(node.InnerText);
            s.Append(' ');
         }
         return s.ToString().Trim();
      }

      /// <summary>
      ///   Returns the text of the <see cref="Annotation"/> with special elements
      ///   evaluated in the context of the <see cref="XPathNavigator"/>.
      /// </summary>
      /// <param name="instance">
      ///   An <see cref="XPathNavigator"/> that provides the context for the 
      ///   special elements.
      /// </param>
      /// <returns>
      ///   A textual representation of the <see cref="Annotation"/>.
      /// </returns>
      /// <remarks>
      ///   The elements <c>&lt;name></c> and <c>&lt;value-of></c> are processed.
      /// </remarks>
      public string ToString(XPathNavigator instance)
      {
         return ToString(instance, null);
      }

      /// <summary>
      ///   Returns the text of the <see cref="Annotation"/> with special elements
      ///   evaluated in the context of the <see cref="XPathNavigator"/>.
      /// </summary>
      /// <param name="instance">
      ///   An <see cref="XPathNavigator"/> that provides the context for the 
      ///   special elements.
      /// </param>
      /// <param name="context">
      ///   A query engine context to evaulate the &lt;value-of> element in.  This can be null.
      /// </param>
      /// <returns>
      ///   A textual representation of the <see cref="Annotation"/>.
      /// </returns>
      /// <remarks>
      ///   The elements <c>&lt;name></c> and <c>&lt;value-of></c> are processed.
      /// </remarks>
      public string ToString(XPathNavigator instance, object context)
      {
         StringBuilder s = new StringBuilder();
         foreach (XmlNode node in this)
         {
            if (node.LocalName == "name")
            {
               // TODO: path attribute
               s.Append(XPathHelper.FullName(instance));
            }
            else if (node.LocalName == "value-of")
            {
               XsltContext xcontext = context as XsltContext;
               XPathExpression expression = XPathExpression.Compile(node.Attributes["select"].Value);
               expression.SetContext(xcontext);
               object result = instance.Evaluate(expression);
               XPathNodeIterator ni = result as XPathNodeIterator;
               if (ni != null)
               {
                  if (ni.MoveNext())
                     s.Append(ni.Current.Value);
               }
               else
               {
                     s.Append(result.ToString());
               }
            }
            else
               s.Append(node.InnerText);
         }
         return s.ToString().Trim();
      }

   }
}
