using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml.XPath;

namespace Sepia.Schematron
{
   /// <summary>
   ///   Helper methods for <see cref="System.Xml.XPath"/>.
   /// </summary>
   public static class XPathHelper
   {
      /// <summary>
      ///   Gets the full name of the current node.
      /// </summary>
      /// <param name="navigator"></param>
      /// <returns></returns>
      public static string FullName(XPathNavigator navigator)
      {
         StringBuilder s = new StringBuilder();
         FullName(navigator.Clone(), s);
         return s.ToString();
      }

      static void FullName(XPathNavigator navigator, StringBuilder s)
      {
         if (navigator.NodeType == XPathNodeType.Root)
            return;

         string name = navigator.Name;
         string value = null;
         XPathNodeType nodeType = navigator.NodeType;
         if (nodeType == XPathNodeType.Attribute)
            value = navigator.Value;

         int same = 0;
         int position = 0;
         XPathNavigator sibling = navigator.Clone();
         sibling.MoveToFirst();
         do
         {
            if (sibling.NodeType == nodeType && sibling.Name == name)
            {
               if (sibling.IsSamePosition(navigator))
                  position = same;
               else
                  ++same;
            }
         } while (sibling.MoveToNext());

         if (navigator.MoveToParent())
            FullName(navigator, s);

         switch (nodeType)
         {
            case XPathNodeType.Element:
               s.Append('/');
               s.Append(name);
               if (same != 0)
               {
                  s.Append('[');
                  s.Append((position + 1).ToString(CultureInfo.InvariantCulture));
                  s.Append(']');
               }
               break;
            case XPathNodeType.Attribute:
               s.Append("[@");
               s.Append(name);
               if (same != 0)
                  s.AppendFormat(" = '{0}'", value);
               s.Append("]");
               break;

            case XPathNodeType.Comment:
            case XPathNodeType.Namespace:
            case XPathNodeType.ProcessingInstruction:
            case XPathNodeType.Root:
            case XPathNodeType.Text:
            case XPathNodeType.Whitespace:
            default:
               throw new NotSupportedException();
         }

      }
   }
}
