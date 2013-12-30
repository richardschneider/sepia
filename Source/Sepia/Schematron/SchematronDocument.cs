using Common.Logging;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

// TODO: foreign elements
// TODO: inclusion
  
namespace Sepia.Schematron
{
    using Sepia.Schematron;

    /// <summary>
   ///   A Schematron schema.
   /// </summary>
   /// <remarks>
   ///   <para>
   ///   The <see cref="SchematronDocument"/> is defined by ISO/IEC 19757-3 
   ///   "Document Schema Definition Languages (DSDL) - Part 3: Rule-based validation — Schematron".
   ///   </para>
   ///   <para>
   ///   A <b>SchematronDocument</b> is used to validate that a structured document (XML) conforms to the specified <see cref="Pattern.Rules"/>.
   ///   </para>
   ///   <para>
   ///   Schematron has two characteristic high-level abstractions: the <see cref="Pattern"/> and the <see cref="Phase"/>. These
   ///   allow the representation of non-regular, non-sequential constraints that ISO/IEC 19757-2 and W3C WSD cannot specify, and
   ///   various dynamic or contingent constraints.
   ///   </para>
   /// <para>
   ///   The <see cref="SchematronValidator"/> class is used the validate an <see cref="XmlDocument"/> against a Schematron schema.
   /// </para>
   /// </remarks>
   /// <seealso cref="SchematronValidator"/>
   /// <seealso cref="ValidationReport"/>
   [Serializable]
   public sealed class SchematronDocument
   {
       static ILog log = LogManager.GetLogger(typeof(SchematronDocument));

      private Annotation title;
      private NamespaceDefinitionCollection namespaces;
      private Annotation annotation;
      private PhaseCollection phases;
      private PatternCollection patterns;
      private DiagnosticCollection diagnostics;
      private NameValueCollection parameters;
      private string id;
      private string fpi;
      private string schemaVersion;
      private string defaultPhase = Phase.All;
      private string icon;
      private string see;
      private string queryLanguage = Schematron.DefaultQueryLanguage;

      SchematronDocument compiledDocument;

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
      ///   The namespace prefixes and URIs used by the <see cref="SchematronDocument"/>.
      /// </summary>
      /// <value>
      ///   A <see cref="NamespaceDefinitionCollection"/> containing the <see cref="NamespaceDefinition"/> objects.
      /// </value>
      /// <remarks>
      ///   A <see cref="NamespaceDefinition.Prefix"/> can be used in a <see cref="Rule.Context"/> or 
      ///   <see cref="Assertion.Test"/>.
      /// </remarks>
      public NamespaceDefinitionCollection Namespaces
      {
         get
         {
            if (namespaces == null)
               namespaces = new NamespaceDefinitionCollection();
            
            return namespaces;
         }
         set
         {
            namespaces = value;
         }
      }

      /// <summary>
      ///   Determines if any <see cref="Namespaces"/> have been specified.
      /// </summary>
      /// <seealso cref="Namespaces"/>
      public bool HasNamespaces
      {
         get { return namespaces != null && namespaces.Count > 0; }
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
      ///   A named collection of <see cref="Pattern"/> objects.
      /// </summary>
      /// <value>
      ///   A <see cref="PhaseCollection"/>, the default value is an empty collection.
      /// </value>
      /// <remarks>
      ///   A <see cref="Phase"/> determines which <see cref="Patterns"/> to run.
      /// </remarks>
      /// <seealso cref="HasPhases"/>
      /// <seealso cref="DefaultPhase"/>
      /// <seealso cref="Patterns"/>
      public PhaseCollection Phases
      {
         get
         {
            if (phases == null)
               phases = new PhaseCollection();

            return phases;
         }
         set
         {
            phases = value;
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
      ///   Determines if any <see cref="Phases"/> have been specified.
      /// </summary>
      /// <seealso cref="Phases"/>
      public bool HasPhases
      {
         get { return phases != null && phases.Count > 0; }
      }

      /// <summary>
      ///   A collection of <see cref="Pattern"/> objects.
      /// </summary>
      /// <value>
      ///   A <see cref="PatternCollection"/>, the default value is an empty collection.
      /// </value>
      /// <remarks>
      ///   Each <see cref="Pattern"/> is a lexically-ordered collection of <see cref="Pattern.Rules"/>.
      /// </remarks>
      /// <seealso cref="Pattern"/>
      /// <seealso cref="Rule"/>
      public PatternCollection Patterns
      {
         get
         {
            if (patterns == null)
               patterns = new PatternCollection();

            return patterns;
         }
         set
         {
            patterns = value;
         }
      }

      /// <summary>
      ///   A collection of <see cref="Diagnostic"/> objects.
      /// </summary>
      /// <value>
      /// </value>
      /// <remarks>
      /// Each <see cref="Diagnostic"/> is a named natural language statement providing information to end-users of 
      /// a <see cref="SchematronValidator"/> concerning the expected and actual values together with repair hints.
      /// </remarks>
      /// <seealso cref="Diagnostic"/>
      /// <seealso cref="HasDiagnostics"/>
      public DiagnosticCollection Diagnostics
      {
         get
         {
            if (diagnostics == null)
               diagnostics = new DiagnosticCollection();

            return diagnostics;
         }
         set
         {
            diagnostics = value;
         }
      }

      /// <summary>
      ///   Determines if any <see cref="Diagnostics"/> have been specified.
      /// </summary>
      /// <seealso cref="Diagnostic"/>
      public bool HasDiagnostics
      {
         get { return diagnostics != null && diagnostics.Count > 0; }
      }

      /// <summary>
      ///   Gets or ses the name of the query language used in a <see cref="Rule.Context"/> or <see cref="Assertion.Test"/>.
      /// </summary>
      /// <value>
      ///   The case-insesitive name of a query language.  The default value is <see cref="Schematron.DefaultQueryLanguage"/>.
      /// </value>
      /// <remarks>
      ///   The <b>QueryLanguage</b> is bound to an <see cref="Sepia.Schematron.Queries.IQueryLanguage"/> via
      ///   the <see cref="Sepia.Schematron.Queries.QueryLanguageFactory"/> found at <see cref="Schematron.QueryLanguages"/>.
      /// </remarks>
      /// <seealso cref="Schematron.DefaultQueryLanguage"/>
      public string QueryLanguage
      {
         get { return queryLanguage; }
         set { queryLanguage = value; }
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
      ///   The version of the <see cref="SchematronDocument"/>.
      /// </summary>
      public string SchemaVersion
      {
         get
         {
            return schemaVersion;
         }
         set
         {
            schemaVersion = value;
         }
      }

      /// <summary>
      /// Indicates the default <see cref="Phase"/> to use when validating.
      /// </summary>
      /// <value>
      ///   The name of default <see cref="Phase"/> to use when validating.  The default value is "#ALL", which indicates
      ///   all <see cref="Patterns"/> should be used by default.
      /// </value>
      public string DefaultPhase
      {
         get
         {
            return this.defaultPhase;
         }
         set
         {
            this.defaultPhase = value;
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
      ///   Gets the compiled form of the <see cref="SchematronDocument"/>.
      /// </summary>
      /// <value>
      ///   A "minimal syntax" <see cref="SchematronDocument"/>, as specified in Section 6.2 of ISO/IEC 19757-3.
      /// </value>
      /// <seealso cref="Compiler"/>
      public SchematronDocument CompiledDocument
      {
         get
         {
            if (compiledDocument == null)
            {
               Compiler compiler = new Compiler();
               compiledDocument = compiler.Compile(this);
            }

            return compiledDocument;
         }
      }

      #region Saving
      /// <summary>
      ///   Saves the <see cref="SchematronDocument"/> to the specified path.
      /// </summary>
      /// <param name="path">The filename to save the document to.</param>
      public void Save(string path)
      {
         SchematronWriter w = new SchematronWriter();
         w.WriteDocument(this, path);
      }

      /// <summary>
      ///   Saves the <see cref="SchematronDocument"/> to the specified <see cref="TextWriter"/>.
      /// </summary>
      /// <param name="writer">The <see cref="TextWriter"/> used to save the document.</param>
      public void Save(TextWriter writer)
      {
         SchematronWriter w = new SchematronWriter();
         w.WriteDocument(this, writer);
      }

      /// <summary>
      ///   Saves the <see cref="SchematronDocument"/> to the specified <see cref="Stream"/>.
      /// </summary>
      /// <param name="s">The <see cref="Stream"/> used to save the document.</param>
      public void Save(Stream s)
      {
         SchematronWriter w = new SchematronWriter();
         w.WriteDocument(this, s);
      }

      /// <summary>
      ///   Saves the <see cref="SchematronDocument"/> to the specified <see cref="StringBuilder"/>.
      /// </summary>
      /// <param name="s">The <see cref="StringBuilder"/> used to save the document.</param>
      public void Save(StringBuilder s)
      {
         SchematronWriter w = new SchematronWriter();
         w.WriteDocument(this, s);
      }
      #endregion

      #region Loading
      void Reset()
      {
         this.Annotation = null;
         this.compiledDocument = null;
         this.Diagnostics = null;
         this.Namespaces = null;
         this.Patterns = null;
         this.Phases = null;
         this.Title = null;
      }

      /// <summary>
      ///   Loads the <see cref="SchematronDocument"/> from the specified <see cref="StringBuilder"/>.
      /// </summary>
      /// <param name="s">The <see cref="StringBuilder"/> containing the document to load.</param>
      /// <remarks>
      ///   The following checks are performed on the document:
      ///   <list type="bullet">
      ///   <item>It is a well-formed XML document.</item>
      ///   <item>The <see cref="XmlDocument.DocumentElement"/> namespace is a <see cref="Schematron"/> namespace.</item>
      ///   <item>The document validates against the W3C XSD schema for a <see cref="SchematronDocument"/>.</item>
      ///   <item>The document validates against the Schematron schema for a <see cref="SchematronDocument"/>.</item>
      ///   </list>
      ///  <para>
      ///   <b>Load</b> accepts both "http://purl.oclc.org/dsdl/schematron" and "http://www.ascc.net/xml/schematron"
      ///   as valid namespaces.
      /// </para>
      /// </remarks>
      public void Load(StringBuilder s)
      {
         Reset();

         if (s == null)
            throw new ArgumentNullException("s");

        SchematronReader reader = new SchematronReader();
        using (XmlReader xmlReader = XmlReader.Create(new StringReader(s.ToString()), reader.CreateXmlReaderSettings()))
        {
            reader.ReadSchema(this, xmlReader, true);
        }
         
      }

      /// <summary>
      ///   Loads the <see cref="SchematronDocument"/> from the specified <see cref="Stream"/>.
      /// </summary>
      /// <param name="s">The <see cref="Stream"/> containing the document to load.</param>
      /// <remarks>
      ///   The following checks are performed on the document:
      ///   <list type="bullet">
      ///   <item>It is a well-formed XML document.</item>
      ///   <item>The <see cref="XmlDocument.DocumentElement"/> namespace is a <see cref="Schematron"/> namespace.</item>
      ///   <item>The document validates against the W3C XSD schema for a <see cref="SchematronDocument"/>.</item>
      ///   <item>The document validates against the Schematron schema for a <see cref="SchematronDocument"/>.</item>
      ///   </list>
      ///  <para>
      ///   <b>Load</b> accepts both "http://purl.oclc.org/dsdl/schematron" and "http://www.ascc.net/xml/schematron"
      ///   as valid namespaces.
      /// </para>
      /// </remarks>
      public void Load(Stream s)
      {
         Reset();

         if (s == null)
            throw new ArgumentNullException("s");

        SchematronReader reader = new SchematronReader();
        using (XmlReader xmlReader = XmlReader.Create(s, reader.CreateXmlReaderSettings()))
        {
            reader.ReadSchema(this, xmlReader, true);
        }
    }

    internal void Load(Stream s, bool doSchematronValidation)
    {
        Reset();

        if (s == null)
            throw new ArgumentNullException("s");

        SchematronReader reader = new SchematronReader();
        using (XmlReader xmlReader = XmlReader.Create(s, reader.CreateXmlReaderSettings()))
        {
            reader.ReadSchema(this, xmlReader, doSchematronValidation);
        }
    }
    
       /// <summary>
      ///   Loads the <see cref="SchematronDocument"/> from the specified <see cref="TextReader"/>.
      /// </summary>
      /// <param name="textReader">The <see cref="TextReader"/> containing the document to load.</param>
      /// <remarks>
      ///   The following checks are performed on the document:
      ///   <list type="bullet">
      ///   <item>It is a well-formed XML document.</item>
      ///   <item>The <see cref="XmlDocument.DocumentElement"/> namespace is a <see cref="Schematron"/> namespace.</item>
      ///   <item>The document validates against the W3C XSD schema for a <see cref="SchematronDocument"/>.</item>
      ///   <item>The document validates against the Schematron schema for a <see cref="SchematronDocument"/>.</item>
      ///   </list>
      ///  <para>
      ///   <b>Load</b> accepts both "http://purl.oclc.org/dsdl/schematron" and "http://www.ascc.net/xml/schematron"
      ///   as valid namespaces.
      /// </para>
      /// </remarks>
      public void Load(TextReader textReader)
      {
         Reset();

         if (textReader == null)
             throw new ArgumentNullException("textReader");

        SchematronReader reader = new SchematronReader();
        using (XmlReader xmlReader = XmlReader.Create(textReader, reader.CreateXmlReaderSettings()))
        {
            reader.ReadSchema(this, xmlReader, true);
        }
    }

      /// <summary>
      ///   Loads the <see cref="SchematronDocument"/> from the specified URI.
      /// </summary>
      /// <param name="uri">The URI containing the document to load.</param>
      /// <remarks>
      ///   The following checks are performed on the document:
      ///   <list type="bullet">
      ///   <item>It is a well-formed XML document.</item>
      ///   <item>The <see cref="XmlDocument.DocumentElement"/> namespace is a <see cref="Schematron"/> namespace.</item>
      ///   <item>The document validates against the W3C XSD schema for a <see cref="SchematronDocument"/>.</item>
      ///   <item>The document validates against the Schematron schema for a <see cref="SchematronDocument"/>.</item>
      ///   </list>
      ///  <para>
      ///   <b>Load</b> accepts both "http://purl.oclc.org/dsdl/schematron" and "http://www.ascc.net/xml/schematron"
      ///   as valid namespaces.
      /// </para>
      /// </remarks>
      public void Load(string uri)
      {
         Reset();

         if (uri == null)
            throw new ArgumentNullException("uri");

        if (log.IsDebugEnabled)
            log.Debug("Loading " + uri);

        SchematronReader reader = new SchematronReader();
        using (XmlReader xmlReader = XmlReader.Create(uri, reader.CreateXmlReaderSettings()))
        {
            reader.ReadSchema(this, xmlReader, true);
        }
      }

      #endregion
   }

}
