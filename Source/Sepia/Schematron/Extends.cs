using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

// TODO: foreign attributes

namespace Sepia.Schematron
{
   /// <remarks/>
   [System.SerializableAttribute()]
   [System.Xml.Serialization.XmlTypeAttribute("extends", AnonymousType = true, Namespace = "http://www.ascc.net/xml/schematron")]
   [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.ascc.net/xml/schematron", IsNullable = false)]
   public sealed class Extends
   {
      private string ruleID;

      /// <remarks/>
      [System.Xml.Serialization.XmlAttributeAttribute("rule", DataType = "IDREF")]
      public string RuleID
      {
         get
         {
            return ruleID;
         }
         set
         {
            ruleID = value;
         }
      }
   }

   /// <summary>
   ///   A list of <see cref="Extends"/> objects.
   /// </summary>
   public class ExtendsCollection : List<Extends>
   {
   }
}
