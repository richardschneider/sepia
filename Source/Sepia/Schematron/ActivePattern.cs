using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

// TODO: foreign elements

namespace Sepia.Schematron
{
   /// <remarks/>
   [System.SerializableAttribute()]
   [System.Xml.Serialization.XmlTypeAttribute("active", AnonymousType = true, Namespace = "http://www.ascc.net/xml/schematron")]
   [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.ascc.net/xml/schematron", IsNullable = false)]
   public sealed class ActivePattern
   {
      private Annotation annotation;
      private string pattern;


      /// <remarks/>
      [System.Xml.Serialization.XmlAttributeAttribute("pattern", DataType = "IDREF")]
      public string Pattern
      {
         get
         {
            return pattern;
         }
         set
         {
            pattern = value;
         }
      }

      /// <summary>
      /// 
      /// </summary>
      [XmlAnyElement]
      [XmlText]
      public Annotation Annotation
      {
         get
         {
            if (annotation == null)
               annotation = new Annotation();
            
            return annotation;
         }
         set
         {
            annotation = value;
         }
      }
   }

   /// <summary>
   ///   A list of <see cref="ActivePattern"/> objects.
   /// </summary>
   public sealed class ActivePatternCollection : List<ActivePattern>
   {
   }
}
