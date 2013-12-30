using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Xml;
using System.Xml.Serialization;

// TODO: foreign element
// TODO: include element

namespace Sepia.Schematron
{

   /// <summary>
   ///   A lexically-ordered collection of rules.
   /// </summary>
   /// <see cref="Rules"/>
   [Serializable()]
   public sealed class Pattern
   {
      private Annotation annotation;
      private RuleCollection rules;
      private Annotation title;
      private NameValueCollection parameters;
      private string see;
      private string id;
      private string icon;
      string fpi;
      bool isAbstract;
      string basePatternID;

      /// <summary>
      ///  Indicates if the <see cref="Pattern"/> is abstract.
      /// </summary>
      public bool IsAbstract
      {
         get { return isAbstract; }
         set { isAbstract = value; }
      }

      /// <summary>
      ///   Gets or sets the <see cref="Pattern.ID"/> of the base <see cref="Pattern"/>.
      /// </summary>
      /// <remarks>
      ///   In schematron, the <b>BasePatternID</b> is defined with the 'is-a' attribute.
      /// </remarks>
      /// <example>
      /// <code>
/// &lt;schema xmlns="http://purl.oclc.org/dsdl/schematron" xml:lang="en" >
///   &lt;title>Test for abstract patterns&lt;/title>
/// 
///   &lt;pattern abstract="true" id="requiredAttribute">
///     &lt;title>Required Attributes&lt;/title>
///     &lt;rule context=" $context ">
///       &lt;assert test="string-length( $attribute ) &gt; 0">
///         The &lt;name/> element should have a &lt;value-of select="$attribute /name()" /> attribute.
///       &lt;/assert>
///     &lt;/rule>
///   &lt;/pattern>
/// 
///   &lt;pattern is-a="requiredAttribute">
///     &lt;param name="context" value="foo" />
///     &lt;param name="attribute" value="@id" />
///   &lt;/pattern>
/// 
///   &lt;pattern is-a="requiredAttribute">
///     &lt;param name="context" value="foo" />
///     &lt;param name="attribute" value="@bar" />
///   &lt;/pattern>
/// 
      /// &lt;/schema>
      /// </code>
      /// </example>
      public string BasePatternID
      {
         get { return basePatternID; }
         set { basePatternID = value; }
      }

      /// <summary>
      ///  A collection of the <see cref="Rule"/> objects.
      /// </summary>
      /// <value>
      ///   A <see cref="RuleCollection"/>.  The default value is an empty collection.
      /// </value>
      public RuleCollection Rules
      {
         get
         {
            if (rules == null)
               rules = new RuleCollection();

            return rules;
         }
         set
         {
            rules = value;
         }
      }

      /// <summary>
      ///   The parameters for the <see cref="Pattern"/>.
      /// </summary>
      /// <value>
      ///   A <see cref="NameValueCollection"/>.  The default value is an empty collection.
      /// </value>
      /// <seealso cref="HasParameters"/>
      /// <remarks>
      ///   Each parameter value is considered an expression and is evaluated by query language.  Thus, to use
      ///   a string constant the value should be enclosed in quotation marks.
      /// </remarks>
      public NameValueCollection Parameters
      {
         get
         {
            if (parameters == null)
               parameters = new NameValueCollection();

            return parameters;
         }
         set
         {
            parameters = value;
         }
      }

      /// <summary>
      ///   Determines if any <see cref="Parameters"/> have been specified.
      /// </summary>
      /// <seealso cref="Parameters"/>
      public bool HasParameters
      {
         get { return parameters != null && parameters.Count > 0; }
      }


      /// <summary>
      ///   ISO/IEC Schematron has changed the 'name' attribute into the <see cref="Title"/> element.
      /// </summary>
      [Obsolete("Use the Title property.")]
      public string Name
      {
         get
         {
            return Title.ToString();
         }
         set
         {
            XmlDocument doc = new XmlDocument();
            
            Annotation name = new Annotation();
            name.Add(doc.CreateTextNode(value));
            Title = name;
         }
      }

      /// <summary>
      ///   A summary of the purpose or role of the schema, for the purpose of documentation or a rich user interface.
      /// </summary>
      public Annotation Title
      {
         get
         {
            if (title == null)
               title = new Annotation();

            return title;
         }
         set
         {
            title = value;
         }
      }

      /// <summary>
      ///   Gets or sets the unique identifier.
      /// </summary>
      public string ID
      {
         get
         {
            return id;
         }
         set
         {
            id = value;
         }
      }

      /// <summary>
      ///   A summary of the purpose or role of the <see cref="SchematronDocument"/>, for the purpose of documentation or a rich user interface.
      /// </summary>
      /// <value>
      ///   The default value is an empty <see cref="Annotation"/>.
      /// </value>
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
      /// <summary>
      ///   A formal public identifier for the object.
      /// </summary>
      public string Fpi
      {
         get
         {
            return fpi;
         }
         set
         {
            fpi = value;
         }
      }

      /// <summary>
      ///   The URI of external information of interest to maintainers and users of the schema.
      /// </summary>
      public string See
      {
         get
         {
            return see;
         }
         set
         {
            see = value;
         }
      }

      /// <summary>
      ///  The URI of an image containing some visible representation of the severity, significance or other grouping
      ///  of the associated object.
      /// </summary>
      public string Icon
      {
         get
         {
            return icon;
         }
         set
         {
            icon = value;
         }
      }
   }

   /// <summary>
   ///   A list of <see cref="Pattern"/> objects.
   /// </summary>
   public class PatternCollection : List<Pattern>
   {
      /// <summary>
      ///   Get the <see cref="Pattern"/> with the specified <see cref="Pattern.ID"/>.
      /// </summary>
      /// <param name="id">
      ///   The unique identifier of the <see cref="Pattern"/> to get.
      /// </param>
      /// <returns></returns>
      public Pattern this[string id]
      {
         get
         {
            foreach (Pattern pattern in this)
            {
               if (pattern.ID == id)
                  return pattern;
            }

            throw new ArgumentException(String.Format("'{0}' is not a defined pattern.", id));
         }
      }

   }
}
