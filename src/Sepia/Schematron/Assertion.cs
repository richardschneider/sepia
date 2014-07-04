using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

// TODO: foreign elements

namespace Sepia.Schematron
{

   /// <summary>
   ///   An assertion made about the <see cref="Rule.Context"/> nodes.
   /// </summary>
   /// <example>
   /// <code>
   /// &lt;schema xmlns="http://purl.oclc.org/dsdl/schematron" xml:lang="en" >
   ///   &lt;title>Doggie by Rick Jelliffe and modified by Richard Schneider&lt;/title>
   /// 
   ///   &lt;pattern>
   ///     &lt;title>Doggie&lt;/title>
   ///     &lt;rule context="dog">
   ///       &lt;assert test="count(ear) = 2" >A 'dog' element should contain two 'ear' elements.&lt;/assert>
   ///       &lt;assert test="bone">Every dog needs a bone.&lt;/assert>
   ///       &lt;assert test="nose or @exceptional='true'">A dog should have a nose.&lt;/assert>
   ///     &lt;/rule>
   ///   &lt;/pattern>
   ///   
   /// &lt;/schema>
   /// </code>
   /// </example>
   /// <seealso cref="Rule"/>
   public class Assertion
   {
      private string test;
      private string role;
      private string id;
      private string diagnostics;
      string fpi;
      string icon;
      string see;
      string flag;
      private string subject = ".";
      private Annotation message;
      private object queryExpression;

      /// <summary>
      ///   Gets or sets the query expression for the <see cref="Assertion"/>.
      /// </summary>
      /// <remarks>
      ///   <b>QueryExpression</b> provides storage for the <see cref="Sepia.Schematron.Queries.IQueryLanguage"/> implementation.
      /// </remarks>
      public object QueryExpression
      {
         get { return queryExpression; }
         set { queryExpression = value; }
      }

      /// <summary>
      ///   Gets or sets the message content.
      /// </summary>
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

      /// <summary>
      ///   A boolean query.
      /// </summary>
      /// <value>
      ///   The string representation of an expression in the <see cref="SchematronDocument.QueryLanguage"/>.
      /// </value>
      public string Test
      {
         get
         {
            return test;
         }
         set
         {
            test = value;
         }
      }

      /// <summary>
      ///   A name describing the function of the <see cref="Rule.Context"/> in the <see cref="Pattern"/>.
      /// </summary>
      public string Role
      {
         get
         {
            return role;
         }
         set
         {
            role = value;
         }
      }

      /// <summary>
      ///   A path allowing more precise specification of nodes. 
      /// </summary>
      public string Subject
      {
         get
         {
            return subject;
         }
         set
         {
            subject = value;
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
      ///   References to any diagnostics.
      /// </summary>
      public string Diagnostics
      {
         get
         {
            return diagnostics;
         }
         set
         {
            diagnostics = value;
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

      /// <summary>
      ///   The name of a boolean flag variable. The value of a flag becomes true when the <see cref="Assertion"/> fails.
      /// </summary>
      public string Flag
      {
         get { return flag; }
         set { flag = value; }
      }
   }

   /// <summary>
   ///   A list of <see cref="Assertion"/> objects.
   /// </summary>
   public class AssertionCollection : List<Assertion>
   {
   }
}
