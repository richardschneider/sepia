namespace Sepia.Schematron
{
    using Sepia.Schematron;
    using Common.Logging;
    using System;
    using System.Xml;
    using System.Xml.Schema;

    /// <summary>
   ///   Reads a Schematron document and returns its object representation.
   /// </summary>
   public class SchematronReader
   {
      private static ILog log = LogManager.GetLogger(typeof(SchematronReader));

      XmlDocument xml;
      XmlNameTable names;
      string schematronNamespaceURI;

      string defaultPhase;
      string fpi;
      string icon;
      string id;
      string queryBinding;
      string schemaVersion;
      string see;
      string title;
      string ns;
      string pattern;
      string diagnostic;
      string phase;
      string prefix;
      string uri;
      string p;
      string active;
      string context;
      string extends;
      string abstractName;
      string role;
      string rule;
      string subject;
      string test;
      string name;
      string assert;
      string report;
      string diagnostics;
      string param;
      string value;
      string isa;
      string let;
      string flag;

       internal SchematronReader()
       {
           names = new NameTable();
           defaultPhase = names.Add("defaultPhase");
           fpi = names.Add("fpi");
           icon = names.Add("icon");
           id = names.Add("id");
           queryBinding = names.Add("queryBinding");
           schemaVersion = names.Add("schemaVersion");
           see = names.Add("see");
           title = names.Add("title");
           ns = names.Add("ns");
           pattern = names.Add("pattern");
           diagnostic = names.Add("diagnostic");
           phase = names.Add("phase");
           prefix = names.Add("prefix");
           uri = names.Add("uri");
           p = names.Add("p");
           active = names.Add("active");
           context = names.Add("context");
           extends = names.Add("extends");
           abstractName = names.Add("abstract");
           role = names.Add("role");
           rule = names.Add("rule");
           subject = names.Add("subject");
           test = names.Add("test");
           name = names.Add("name");
           assert = names.Add("assert");
           report = names.Add("report");
           diagnostics = names.Add("diagnostics");
           param = names.Add("param");
           value = names.Add("value");
           isa = names.Add("is-a");
           let = names.Add("let");
           flag = names.Add("flag");
       }

       internal void ReadSchema(SchematronDocument schematron, XmlReader xmlReader, bool validateWithSchematron)
       {
           this.xml = new XmlDocument();
           this.xml.Load(xmlReader);

           // Check that the namespace is a schematron namespace.
           XmlElement e = xml.DocumentElement;
           if (e.NamespaceURI != Schematron.IsoNamespace && e.NamespaceURI != Schematron.OriginalNamespace)
               throw new Exception(String.Format("'{0}' is not a namepsace for schematron document."));
           schematronNamespaceURI = e.NamespaceURI;

           // Check that the document validates against the Schematron schema.
           if (validateWithSchematron)
           {
               SchematronValidator validator = new SchematronValidator();
               if (e.NamespaceURI == Schematron.OriginalNamespace)
               {
                   validator.SchemaDocument = Schematron.Default.OriginalSchematronSchema;
                   validator.ValidationPhase = "Full";
               }
               else if (e.NamespaceURI == Schematron.IsoNamespace)
               {
                   validator.SchemaDocument = Schematron.Default.IsoSchematronSchema;
               }
               validator.Validate(xml);
           }

           // Read the document element and all children.
           schematron.ID = e.GetAttribute(id);
           schematron.Fpi = e.GetAttribute(fpi);
           schematron.Icon = e.GetAttribute(icon);
           schematron.See = e.GetAttribute(see);
           schematron.SchemaVersion = e.GetAttribute(schemaVersion);
           if (e.HasAttribute(defaultPhase)) schematron.DefaultPhase = e.GetAttribute(defaultPhase);
           if (e.HasAttribute(queryBinding)) schematron.QueryLanguage = e.GetAttribute(queryBinding);

           if (e.HasChildNodes)
           {
               foreach (XmlNode childNode in e.ChildNodes)
               {
                   XmlElement child = childNode as XmlElement;
                   if (child != null && child.NamespaceURI == schematronNamespaceURI)
                   {
                       if (child.LocalName == ns)
                           schematron.Namespaces.Add(ReadNamespaceDefinition(child));
                       else if (child.LocalName == pattern)
                           schematron.Patterns.Add(ReadPattern(child));
                       else if (child.LocalName == phase)
                           schematron.Phases.Add(ReadPhase(child));
                       else if (child.LocalName == diagnostics)
                           schematron.Diagnostics = ReadDiagnostics(child);
                       else if (child.LocalName == p)
                           schematron.Annotation.Add(child);
                       else if (child.LocalName == title)
                           schematron.Title = ReadAnnotation(child);
                       else if (child.LocalName == p)
                           schematron.Annotation.Add(child);
                       else if (child.LocalName == let)
                           schematron.Parameters.Add(child.GetAttribute(name), child.GetAttribute(value));
                       else
                           throw new Exception(String.Format("'{0}' is an unknown schematron element.", child.Name));
                   }
               }
           }
       }

      internal SchematronDocument ReadSchema(SchematronDocument schematron, bool validateWithSchematron)
      {
         // Check that the namespace is a schematron namespace.
         XmlElement e = xml.DocumentElement;
         if (e.NamespaceURI != Schematron.IsoNamespace && e.NamespaceURI != Schematron.OriginalNamespace)
            throw new Exception(String.Format("'{0}' is not a namepsace for schematron document."));
         schematronNamespaceURI = e.NamespaceURI;

         // Check that the document validates against the W3C XSD schema.
         xml.Schemas = Schematron.Default.XsdSet;
         xml.Validate(null);

         // Check that the document validates against the Schematron schema.
         if (validateWithSchematron)
         {
            SchematronValidator validator = new SchematronValidator();
            if (e.NamespaceURI == Schematron.OriginalNamespace)
            {
               validator.SchemaDocument = Schematron.Default.OriginalSchematronSchema;
               validator.ValidationPhase = "Full";
            }
            else if (e.NamespaceURI == Schematron.IsoNamespace)
            {
               validator.SchemaDocument = Schematron.Default.IsoSchematronSchema;
            }
            validator.Validate(xml);
         }

         // Read the document element and all children.
         schematron.ID = e.GetAttribute(id);
         schematron.Fpi = e.GetAttribute(fpi);
         schematron.Icon = e.GetAttribute(icon);
         schematron.See = e.GetAttribute(see);
         schematron.SchemaVersion = e.GetAttribute(schemaVersion);
         if (e.HasAttribute(defaultPhase)) schematron.DefaultPhase = e.GetAttribute(defaultPhase);
         if (e.HasAttribute(queryBinding)) schematron.QueryLanguage = e.GetAttribute(queryBinding);

         if (e.HasChildNodes)
         {
            foreach (XmlNode childNode in e.ChildNodes)
            {
               XmlElement child = childNode as XmlElement;
               if (child != null &&  child.NamespaceURI == schematronNamespaceURI)
               {
                  if (child.LocalName == ns)
                     schematron.Namespaces.Add(ReadNamespaceDefinition(child));
                  else if (child.LocalName == pattern)
                     schematron.Patterns.Add(ReadPattern(child));
                  else if (child.LocalName == phase)
                     schematron.Phases.Add(ReadPhase(child));
                  else if (child.LocalName == diagnostics)
                     schematron.Diagnostics = ReadDiagnostics(child);
                  else if (child.LocalName == p)
                     schematron.Annotation.Add(child);
                  else if (child.LocalName == title)
                     schematron.Title = ReadAnnotation(child);
                  else if (child.LocalName == p)
                     schematron.Annotation.Add(child);
                  else if (child.LocalName == let)
                     schematron.Parameters.Add(child.GetAttribute(name), child.GetAttribute(value));
                  else
                     throw new Exception(String.Format("'{0}' is an unknown schematron element.", child.Name));
               }
            }
         }

         return schematron;
      }

      DiagnosticCollection ReadDiagnostics(XmlElement e)
      {
         DiagnosticCollection diags = new DiagnosticCollection();

         if (e.HasChildNodes)
         {
            foreach (XmlNode childNode in e.ChildNodes)
            {
               XmlElement child = childNode as XmlElement;
               if (child != null && child.NamespaceURI == schematronNamespaceURI)
               {
                  if (child.LocalName == diagnostic)
                     diags.Add(ReadDiagnostic(child));
                  else
                     throw new Exception(String.Format("'{0}' is an unknown schematron element.", child.Name));
               }
            }
         }

         return diags;
      }

      Diagnostic ReadDiagnostic(XmlElement e)
      {
         Diagnostic diagnostic = new Diagnostic();

         diagnostic.ID = e.GetAttribute(id);
         diagnostic.Fpi = e.GetAttribute(fpi);
         diagnostic.Icon = e.GetAttribute(icon);
         diagnostic.See = e.GetAttribute(see);

         if (e.HasChildNodes)
         {
            foreach (XmlNode node in e.ChildNodes)
            {
               diagnostic.Message.Add(node);
            }
         }

         return diagnostic;
      }

      Annotation ReadAnnotation(XmlElement e)
      {
         Annotation annotation = new Annotation();

         if (e.HasChildNodes)
         {
            foreach (XmlNode node in e.ChildNodes)
            {
               annotation.Add(node);
            }
         }

         return annotation;
      }


      Pattern ReadPattern(XmlElement e)
      {
         Pattern pattern = new Pattern();

         pattern.ID = e.GetAttribute(id);
         pattern.Fpi = e.GetAttribute(fpi);
         pattern.Icon = e.GetAttribute(icon);
         pattern.See = e.GetAttribute(see);
#pragma warning disable 612,618
         pattern.Name = e.GetAttribute(name);
#pragma warning restore 612,618

          pattern.BasePatternID = e.GetAttribute(isa);
         if (e.HasAttribute(abstractName)) pattern.IsAbstract = XmlConvert.ToBoolean(e.GetAttribute(abstractName));

         if (e.HasChildNodes)
         {
            foreach (XmlNode childNode in e.ChildNodes)
            {
               XmlElement child = childNode as XmlElement;
               if (child != null && child.NamespaceURI == schematronNamespaceURI)
               {
                  if (child.LocalName == title)
                     pattern.Title = ReadAnnotation(child);
                  else if (child.LocalName == rule)
                     pattern.Rules.Add(ReadRule(child));
                  else if (child.LocalName == p)
                     pattern.Annotation.Add(child);
                  else if (child.LocalName == param)
                  {
                     pattern.Parameters.Add(child.GetAttribute(name), child.GetAttribute(value));
                  }
                  else
                     throw new Exception(String.Format("'{0}' is an unknown schematron element.", child.Name));
               }
            }
         }

         return pattern;
      }

      Phase ReadPhase(XmlElement e)
      {
         Phase phase = new Phase();

         phase.ID = e.GetAttribute(id);
         phase.Fpi = e.GetAttribute(fpi);
         phase.Icon = e.GetAttribute(icon);
         phase.See = e.GetAttribute(see);

         if (e.HasChildNodes)
         {
            foreach (XmlNode childNode in e.ChildNodes)
            {
               XmlElement child = childNode as XmlElement;
               if (child != null && child.NamespaceURI == schematronNamespaceURI)
               {
                  if (child.LocalName == active)
                     phase.ActivePatterns.Add(ReadActivePattern(child));
                  else if (child.LocalName == p)
                     phase.Annotation.Add(child);
                  else if (child.LocalName == let)
                     phase.Parameters.Add(child.GetAttribute(name), child.GetAttribute(value));
                  else
                     throw new Exception(String.Format("'{0}' is an unknown schematron element.", child.Name));
               }
            }
         }

         return phase;
      }

      ActivePattern ReadActivePattern(XmlElement e)
      {
         ActivePattern activePattern = new ActivePattern();

         activePattern.Pattern = e.GetAttribute(pattern);

         if (e.HasChildNodes)
         {
            foreach (XmlNode node in e.ChildNodes)
            {
               activePattern.Annotation.Add(node);
            }
         }

         return activePattern;
      }

      Rule ReadRule(XmlElement e)
      {
         Rule rule = new Rule();

         rule.ID = e.GetAttribute(id);
         rule.Fpi = e.GetAttribute(fpi);
         rule.Icon = e.GetAttribute(icon);
         rule.See = e.GetAttribute(see);
         rule.Context = e.GetAttribute(context);
         rule.Flag = e.GetAttribute(flag);
         if (e.HasAttribute(role)) rule.Role = e.GetAttribute(role);
         if (e.HasAttribute(subject)) rule.Subject = e.GetAttribute(subject);
         rule.IsAbstract = XmlConvert.ToBoolean(e.GetAttribute(abstractName));

         if (e.HasChildNodes)
         {
            foreach (XmlNode childNode in e.ChildNodes)
            {
               XmlElement child = childNode as XmlElement;
               if (child != null && child.NamespaceURI == schematronNamespaceURI)
               {
                  if (child.LocalName == extends)
                     rule.Extends.Add(ReadExtends(child));
                  else if (child.LocalName == assert)
                    rule.Assertions.Add(ReadAssertion(child));
                 else if (child.LocalName == report)
                    rule.Assertions.Add(ReadReport(child));
                 else if (child.LocalName == let)
                 {
                    rule.Parameters.Add(child.GetAttribute(name), child.GetAttribute(value));
                 }
                 else
                     throw new Exception(String.Format("'{0}' is an unknown schematron element.", child.Name));
               }
            }
         }
         return rule;
      }

      Extends ReadExtends(XmlElement e)
      {
         Extends extends = new Extends();

         extends.RuleID = e.GetAttribute("rule");

         return extends;
      }

      Assertion ReadAssertion(XmlElement e)
      {
         return ReadAssertion(e, new Assertion());
      }

      Report ReadReport(XmlElement e)
      {
         return (Report) ReadAssertion(e, new Report());
      }

      Assertion ReadAssertion(XmlElement e, Assertion assertion)
      {
         assertion.ID = e.GetAttribute(id);
         assertion.Fpi = e.GetAttribute(fpi);
         assertion.Icon = e.GetAttribute(icon);
         assertion.See = e.GetAttribute(see);
         assertion.Diagnostics = e.GetAttribute(diagnostics);
         assertion.Test = e.GetAttribute(test);
         assertion.Flag = e.GetAttribute(flag);
         if (e.HasAttribute(role)) assertion.Role = e.GetAttribute(role);
         if (e.HasAttribute(subject)) assertion.Subject = e.GetAttribute(subject);

         if (e.HasChildNodes)
         {
            foreach (XmlNode node in e.ChildNodes)
            {
               assertion.Message.Add(node);
            }
         }

         return assertion;
      }


      NamespaceDefinition ReadNamespaceDefinition(XmlElement e)
      {
         NamespaceDefinition nsDef = new NamespaceDefinition();

         nsDef.Prefix = e.GetAttribute(prefix);
         nsDef.Uri = e.GetAttribute(uri);

         return nsDef;
      }

       internal XmlReaderSettings CreateXmlReaderSettings()
       {
           XmlReaderSettings settings = new XmlReaderSettings
            {
                CheckCharacters = true,
                CloseInput = true,
                ConformanceLevel = ConformanceLevel.Document,
                IgnoreComments = true,
                IgnoreProcessingInstructions = true,
                IgnoreWhitespace = true, 
                DtdProcessing = DtdProcessing.Prohibit,
                ValidationFlags = XmlSchemaValidationFlags.AllowXmlAttributes | XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessIdentityConstraints,
                ValidationType = ValidationType.Schema
            };
           settings.Schemas.Add(Schematron.Default.XsdSet);
           settings.NameTable = names;

           return settings;
       }

      /// <summary>
      ///   Reads a Schematron document and returns its object representation.
      /// </summary>
      /// <param name="uri">
      ///   The URI of the schematron document.
      /// </param>
      /// <returns>
      ///   A <see cref="SchematronDocument"/>
      /// </returns>
      /// <remarks>
      ///   The following checks are performed on the document:
      ///   <list type="bullet">
      ///   <item>It is a well-formed XML document.</item>
      ///   <item>The <see cref="XmlDocument.DocumentElement"/> namespace is a schematron namespace.</item>
      ///   <item>The document validates against the W3C XSD schema for a <see cref="SchematronDocument"/>.</item>
      ///   <item>The document validates against the Schematron schema for a <see cref="SchematronDocument"/>.</item>
      ///   </list>
      ///  <para>
      ///   <b>ReadSchematron</b> accepts both "http://purl.oclc.org/dsdl/schematron" and "http://www.ascc.net/xml/schematron"
      ///   as valid namespaces.
      /// </para>
      /// </remarks>
      public static SchematronDocument ReadSchematron(string uri)
      {
         if (log.IsInfoEnabled)
            log.Info("Reading " + uri);

        SchematronDocument doc = new SchematronDocument();
        doc.Load(uri);
        return doc;
      }

      // TODO: Needs reader methods for Stream, TextReader and XmlReader
   }
}
