using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.XPath;

// TODO: Schematron 1.6 has a key element and ISO/IEC does not.
// TODO: foreign
// TODO: inclusion
namespace Sepia.Schematron
{

   /// <summary>
   ///   A list of assertions tested in a <see cref="Context"/>.
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
   public sealed class Rule
   {
      AssertionCollection assertions;
      ExtendsCollection extends;
      private string context;
      private bool isAbstract;
      private string role;
      string subject;
      private string id;
      private object queryExpression;
      string fpi;
      string see;
      string icon;
      string flag;
      private NameValueCollection parameters;

      /// <summary>
      ///   Only for the <see cref="Compiler"/> and is-a patterns.
      /// </summary>
      /// <returns></returns>
      internal Rule Clone()
      {
         Rule clone = (Rule)this.MemberwiseClone();
         clone.Parameters = null;  // will be copied from the is-a pattern's patterns.
         return clone;
      }

      /// <summary>
      ///   Gets or sets the query expression for the <see cref="Rule"/>.
      /// </summary>
      /// <remarks>
      ///   <b>QueryExpression</b> provides storage for the <see cref="Sepia.Schematron.Queries.IQueryLanguage"/> implementation.
      ///   <para>
      ///   This property is intended to be used by the query language to store the compiled representation of the <see cref="Context"/>.
      /// </para>
      /// </remarks>
      public object QueryExpression
      {
         get { return queryExpression; }
         set { queryExpression = value; }
      }

      /// <summary>
      ///   The other <see cref="Rule">rules</see> that are included in this rule.
      /// </summary>
      /// <value>
      ///   An <see cref="ExtendsCollection"/> containing <see cref="Extends"/> objects.
      /// </value>
      /// <remarks>
      ///   The extended <see cref="Rule"/> must be in the same <see cref="Pattern"/> as this rule and must be
      ///   an <see cref="Rule.IsAbstract">abstact rule</see>.
      /// </remarks>
      /// <example>
      /// <code>
      ///   &lt;sch:pattern>
      ///     &lt;sch:title>Elements&lt;/sch:title>
      ///     &lt;!--Abstract Rules -->
      ///     &lt;sch:rule abstract="true" id="childless">
      ///        &lt;sch:assert test="count(*)=0">The &lt;sch:name/> element should not contain any elements.&lt;/sch:assert>
      ///     &lt;/sch:rule>
      ///     &lt;sch:rule abstract="true" id="empty">
      ///       &lt;sch:extends rule="childless" />
      ///       &lt;sch:assert test="string-length(translate(., ' ', '')) = 0"> The &lt;sch:name/> element should be empty.&lt;/sch:assert>
      ///     &lt;/sch:rule>
      ///   &lt;/sch:pattern>
      /// </code>
      /// </example>
      /// <seealso cref="Parameters"/>
      /// <seealso cref="HasExtensions"/>
      public ExtendsCollection Extends
      {
         get
         {
            if (extends == null)
               extends = new ExtendsCollection();

            return extends;
         }
         set
         {
            extends = value;
         }
      }

      /// <summary>
      ///   Determines if any <see cref="Extends"/> have been specified.
      /// </summary>
      /// <seealso cref="Extends"/>
      public bool HasExtensions
      {
         get { return extends != null && extends.Count > 0; }
      }

      /// <summary>
      ///   The parameters for the <see cref="Rule"/>.
      /// </summary>
      /// <value>
      ///   A <see cref="NameValueCollection"/>.  The default value is an empty collection.
      /// </value>
      /// <seealso cref="HasParameters"/>
      /// <remarks>
      ///   Each parameter value is considered an expression and is evaluated by query language.  Thus, to use
      ///   a string constant the value should be enclosed in quotation marks.
      /// </remarks>
      /// <example>
      /// <code>
      /// &lt;schema xmlns="http://purl.oclc.org/dsdl/schematron" xml:lang="en" >
      ///   &lt;title>Test for rules with a let&lt;/title>
      ///   &lt;pattern>
      ///     &lt;title>Time must military format (HH:MM:SS).&lt;/title>
      ///     &lt;rule context="time">
      ///       &lt;let name="hour" value="number(substring(.,1,2))"/>
      ///       &lt;let name="minute" value="number(substring(.,4,2))"/>
      ///       &lt;let name="second" value="number(substring(.,7,2))"/>
      /// 
      ///       &lt;assert test="string-length(.)=8 and substring(.,3,1)=':' and substring(.,6,1)=':'">
      ///         The time element should contain a time in the format HH:MM:SS.&lt;/assert>
      ///       &lt;assert test="$hour>=0 and $hour&lt;=23">
      ///         The hour (&lt;value-of select="$hour"/>) be a value between 0 and 23.&lt;/assert>
      ///       &lt;assert test="$minute>=0 and $minute&lt;=59">
      ///         The minutes (&lt;value-of select="$minute"/>)must be a value between 0 and 59.&lt;/assert>
      ///       &lt;assert test="$second>=0 and $second&lt;=59">
      ///         The second (&lt;value-of select="$second"/>)must be a value between 0 and 59.&lt;/assert>
      ///     &lt;/rule>
      ///   &lt;/pattern>
      /// &lt;/schema>
      /// </code>
      /// </example>
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
      public bool HasParameters
      {
         get { return parameters != null && parameters.Count > 0; }
      }

      /// <summary>
      ///   A collection of <see cref="Assertion"/> and <see cref="Report"/> objects.
      /// </summary>
      /// <value>
      ///   An <see cref="AssertionCollection"/>.  The default value is an empty collection.
      /// </value>
      /// <remarks>
      ///   <b>Assertions</b> are evaluated against the <see cref="Context"/>.  An <see cref="Assertion"/> fails if the
      ///   the <see cref="Assertion.Test"/> returns <b>false</b>.  A <see cref="Report"/> fails if the
      ///   <see cref="Assertion.Test"/> return <b>true</b>.
      /// <para>
      ///   <b>Assertions</b> are considered if-then-else statements.  If an <see cref="Assertion"/> fails, no other
      ///   assertions will be performed for the <see cref="Rule"/>.
      /// </para>
      /// </remarks>
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
      public AssertionCollection Assertions
      {
         get
         {
            if (assertions == null)
               assertions = new AssertionCollection();

            return assertions;
         }
         set
         {
            assertions = value;
         }
      }

      /// <summary>
      ///   The XML element or other information item that causes the <see cref="Rule"/> to fire.
      /// </summary>
      /// <value>
      ///   The string representation of an expression in the <see cref="SchematronDocument.QueryLanguage"/>.
      /// </value>
      /// <remarks>
      ///   An XML element or other information item used by the <see cref="Assertions"/>.
      ///   A <see cref="Rule"/> is said to fire when an information item matches the <b>Context</b>.
      /// </remarks>
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
      public string Context
      {
         get
         {
            return context;
         }
         set
         {
            context = value;
         }
      }

      /// <summary>
      ///   Determines if the <see cref="Rule"/> is abstract.
      /// </summary>
      /// <value>
      ///   <b>true</b> if the <see cref="Rule"/> is abstract; otherwise, <b>false</b> for a concrete rule.
      /// </value>
      /// <remarks>
      ///   Abstract rules are use to <see cref="Extends">extend</see> other abstract and concrete rules.
      /// <para>
      ///   An abstract rule cannot have a <see cref="Context"/>. It is a list of <see cref="Assertions"/> that will be invoked by other rules
      ///   belonging to the same <see cref="Pattern"/> using <see cref="Extends"/>.
      /// </para>
      /// </remarks>
      /// <example>
      /// <code>
      ///   &lt;sch:pattern>
      ///     &lt;sch:title>Elements&lt;/sch:title>
      ///     &lt;!--Abstract Rules -->
      ///     &lt;sch:rule abstract="true" id="childless">
      ///        &lt;sch:assert test="count(*)=0">The &lt;sch:name/> element should not contain any elements.&lt;/sch:assert>
      ///     &lt;/sch:rule>
      ///     &lt;sch:rule abstract="true" id="empty">
      ///       &lt;sch:extends rule="childless" />
      ///       &lt;sch:assert test="string-length(translate(., ' ', '')) = 0"> The &lt;sch:name/> element should be empty.&lt;/sch:assert>
      ///     &lt;/sch:rule>
      ///   &lt;/sch:pattern>
      /// </code>
      /// </example>
      /// <seealso cref="Parameters"/>
      /// <seealso cref="Extends"/>
      public bool IsAbstract
      {
         get
         {
            return isAbstract;
         }
         set
         {
            isAbstract = value;
         }
      }

      /// <summary>
      ///   A name describing the function of the <see cref="Context"/> in the <see cref="Pattern"/>.
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
      ///   The name of a boolean flag variable. The value of a flag becomes true when the <see cref="Rule"/> fires.
      /// </summary>
      public string Flag
      {
         get { return flag; }
         set { flag = value; }
      }
   }

   /// <summary>
   ///   A list of <see cref="Rule"/> objects.
   /// </summary>
   public class RuleCollection : List<Rule>
   {
      /// <summary>
      ///   Get the <see cref="Rule"/> with the specified <see cref="Rule.ID"/>.
      /// </summary>
      /// <param name="id">
      ///   The unique identifier of the <see cref="Rule"/> to get.
      /// </param>
      /// <returns>
      ///   The <see cref="Rule"/> with the specified <paramref name="id"/>.
      /// </returns>
      public Rule this[string id]
      {
         get
         {
            foreach (Rule rule in this)
            {
               if (rule.ID == id)
                  return rule;
            }

            throw new ArgumentException(String.Format("'{0}' is not a defined rule.", id));
         }
      }

   }
}
