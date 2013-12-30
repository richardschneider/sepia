using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

// TODO: foreign elements
// TODO: inclusion

namespace Sepia.Schematron
{
   /// <remarks/>
   [System.SerializableAttribute()]
   [System.Xml.Serialization.XmlTypeAttribute("diagnostic", AnonymousType = true, Namespace = "http://www.ascc.net/xml/schematron")]
   [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.ascc.net/xml/schematron", IsNullable = false)]
   public partial class Diagnostic
   {
      Annotation message;
      private string see;
      private string id;
      private string icon;
      private string fpi;

      /// <remarks/>
      [XmlAttribute("see", DataType = "anyURI")]
      public string See
      {
         get
         {
            return this.see;
         }
         set
         {
            this.see = value;
         }
      }

      /// <remarks/>
      [XmlAttribute("id", DataType = "ID")]
      public string ID
      {
         get
         {
            return this.id;
         }
         set
         {
            this.id = value;
         }
      }

      /// <remarks/>
      [XmlAttribute("icon", DataType = "anyURI")]
      public string Icon
      {
         get
         {
            return this.icon;
         }
         set
         {
            this.icon = value;
         }
      }

      /// <remarks/>
      [XmlAttributeAttribute("fpi")]
      public string Fpi
      {
         get
         {
            return this.fpi;
         }
         set
         {
            this.fpi = value;
         }
      }

      /// <summary>
      ///   Gets or sets the message content.
      /// </summary>
      [XmlAnyElement]
      [XmlText]
      public Annotation Message
      {
         get
         {
            if (message == null)
               message = new Annotation();

            return message;
         }
         set { message = value; }
      }

   }

   /// <summary>
   ///   A list of <see cref="Diagnostic"/> objects.
   /// </summary>
   public sealed class DiagnosticCollection : List<Diagnostic>
   {
      /// <summary>
      ///   Get the <see cref="Diagnostic"/> with the specified <see cref="Diagnostic.ID"/>.
      /// </summary>
      /// <param name="id">
      ///   The unique identifier of the <see cref="Diagnostic"/> to get.
      /// </param>
      /// <returns>
      ///   The <see cref="Diagnostic"/> with the specified <paramref name="id"/>.
      /// </returns>
      public Diagnostic this[string id]
      {
         get
         {
            foreach (Diagnostic diagnostic in this)
            {
               if (diagnostic.ID == id)
                  return diagnostic;
            }

            throw new ArgumentException(String.Format("'{0}' is not a defined diagnostic.", id));
         }
      }

   }
}