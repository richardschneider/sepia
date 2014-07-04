using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Sepia.Schematron
{
   /// <summary>
   ///   Writes (serialises) a <see cref="SchematronDocument"/>.
   /// </summary>
   public class SchematronWriter
   {
      XmlWriter writer;
      string namespaceURI = Schematron.IsoNamespace;

      const string defaultPhase = "defaultPhase";
      const string fpi = "fpi";
      const string icon = "icon";
      const string id = "id";
      const string queryBinding = "queryBinding";
      const string schemaVersion = "schemaVersion";
      const string see = "see";
      const string title = "title";
      const string ns = "ns";
      const string pattern = "pattern";
      const string diagnostic = "diagnostic";
      const string phase = "phase";
      const string prefix = "prefix";
      const string uri = "uri";
      const string p = "p";
      const string active = "active";
      const string context = "context";
      const string extends = "extends";
      const string abstractName = "abstract";
      const string role = "role";
      const string rule = "rule";
      const string subject = "subject";
      const string test = "test";
      const string name = "name";
      const string assert = "assert";
      const string report = "report";
      const string diagnostics = "diagnostics";
      const string schema = "schema";
      const string param = "param";
      const string value = "value";
      const string isa = "is-a";
      const string let = "let";
      const string flag = "flag";

      /// <summary>
      ///   Writes the <see cref="SchematronDocument"/> to the specified path.
      /// </summary>
      /// <param name="schematron">The <see cref="SchematronDocument"/> to write.</param>
      /// <param name="path">The name of the file to write.</param>
      public void WriteDocument(SchematronDocument schematron, string path)
      {
         using (StreamWriter writer = File.CreateText(path))
         {
            WriteDocument(schematron, writer);
         }
      }

      /// <summary>
      ///   Writes the <see cref="SchematronDocument"/> to the specified <see cref="Stream"/>.
      /// </summary>
      /// <param name="schematron">The <see cref="SchematronDocument"/> to write.</param>
      /// <param name="stream">The <see cref="Stream"/> used to the write the document.</param>
      public void WriteDocument(SchematronDocument schematron, Stream stream)
      {
         WriteDocument(schematron, XmlWriter.Create(stream, Settings));
      }

      /// <summary>
      ///   Writes the <see cref="SchematronDocument"/> to the specified <see cref="StringBuilder"/>.
      /// </summary>
      /// <param name="schematron">The <see cref="SchematronDocument"/> to write.</param>
      /// <param name="s">The <see cref="StringBuilder"/> used to the write the document.</param>
      public void WriteDocument(SchematronDocument schematron, StringBuilder s)
      {
         WriteDocument(schematron, XmlWriter.Create(s, Settings));
      }

      /// <summary>
      ///   Writes the <see cref="SchematronDocument"/> to the specified <see cref="TextWriter"/>.
      /// </summary>
      /// <param name="schematron">The <see cref="SchematronDocument"/> to write.</param>
      /// <param name="writer">The <see cref="TextWriter"/> used to the write the document.</param>
      public void WriteDocument(SchematronDocument schematron, TextWriter writer)
      {
         WriteDocument(schematron, XmlWriter.Create(writer, Settings));
      }

      XmlWriterSettings Settings
      {
         get
         {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "   ";
            return settings;
         }
      }

      /// <summary>
      ///   Writes the <see cref="SchematronDocument"/> to the specified <see cref="XmlWriter"/>.
      /// </summary>
      /// <param name="schematron">The <see cref="SchematronDocument"/> to write.</param>
      /// <param name="writer">The <see cref="XmlWriter"/> used to the write the document.</param>
      public void WriteDocument(SchematronDocument schematron, XmlWriter writer)
      {
         if (schematron == null)
            throw new ArgumentNullException("schematron");
         if (writer == null)
            throw new ArgumentNullException("writer");

         this.writer = writer;

         writer.WriteStartDocument();

         WriteSchema(schematron);
         
         writer.WriteEndDocument();
         writer.Flush();
      }

      void WriteSchema(SchematronDocument schematron)
      {
         writer.WriteStartElement(schema, namespaceURI);

         if (!string.IsNullOrEmpty(schematron.ID)) writer.WriteAttributeString(id, schematron.ID);
         if (schematron.DefaultPhase != Phase.All) writer.WriteAttributeString(defaultPhase, schematron.DefaultPhase);
         if (schematron.QueryLanguage != Schematron.DefaultQueryLanguage) writer.WriteAttributeString(queryBinding, schematron.QueryLanguage);
         if (!string.IsNullOrEmpty(schematron.SchemaVersion)) writer.WriteAttributeString(schemaVersion, schematron.SchemaVersion);
         if (!string.IsNullOrEmpty(schematron.Fpi)) writer.WriteAttributeString(fpi, schematron.Fpi);
         if (!string.IsNullOrEmpty(schematron.Icon)) writer.WriteAttributeString(icon, schematron.Icon);
         if (!string.IsNullOrEmpty(schematron.See)) writer.WriteAttributeString(see, schematron.See);

         if (schematron.Title.Count > 0)
            WriteAnnotation(schematron.Title, title);
         WriteAnnotation(schematron.Annotation);
         if (schematron.HasNamespaces)
         {
            foreach (NamespaceDefinition ns in schematron.Namespaces)
               WriteNamespaceDefinition(ns);
         }

         if (schematron.HasParameters)
         {
            foreach (string key in schematron.Parameters)
            {
               writer.WriteStartElement(let, namespaceURI);
               writer.WriteAttributeString(name, key);
               writer.WriteAttributeString(value, schematron.Parameters[key]);
               writer.WriteEndElement();
            }
         }

         if (schematron.HasPhases)
         {
            foreach (Phase p in schematron.Phases)
               WritePhase(p);
         }

         foreach (Pattern p in schematron.Patterns)
            WritePattern(p);

         if (schematron.HasDiagnostics)
         {
            writer.WriteStartElement(diagnostics, namespaceURI);
            foreach (Diagnostic d in schematron.Diagnostics)
               WriteDiagnostic(d);
            writer.WriteEndElement();
         }

         writer.WriteEndElement();
      }

      void WriteAnnotation(Annotation ann)
      {
         foreach (XmlNode node in ann)
         {
            WriteNode(node);
         }
      }

      void WriteAnnotation(Annotation ann, string elementName)
      {
         writer.WriteStartElement(elementName, namespaceURI);
         WriteAnnotation(ann);
         writer.WriteEndElement();
      }

      void WriteNamespaceDefinition(NamespaceDefinition def)
      {
         writer.WriteStartElement(ns, namespaceURI);

         writer.WriteAttributeString(prefix, def.Prefix);
         writer.WriteAttributeString(uri, def.Uri);

         writer.WriteEndElement();
      }

      void WritePhase(Phase p)
      {
         writer.WriteStartElement(phase, namespaceURI);

         if (!string.IsNullOrEmpty(p.ID)) writer.WriteAttributeString(id, p.ID);
         if (!string.IsNullOrEmpty(p.Fpi)) writer.WriteAttributeString(fpi, p.Fpi);
         if (!string.IsNullOrEmpty(p.Icon)) writer.WriteAttributeString(icon, p.Icon);
         if (!string.IsNullOrEmpty(p.See)) writer.WriteAttributeString(see, p.See);

         WriteAnnotation(p.Annotation);
         if (p.HasParameters)
         {
            foreach (string key in p.Parameters)
            {
               writer.WriteStartElement(let, namespaceURI);
               writer.WriteAttributeString(name, key);
               writer.WriteAttributeString(value, p.Parameters[key]);
               writer.WriteEndElement();
            }
         }

         foreach (ActivePattern active in p.ActivePatterns)
            WriteActivePattern(active);

         writer.WriteEndElement();
      }

      void WriteActivePattern(ActivePattern a)
      {
         writer.WriteStartElement(active, namespaceURI);

         writer.WriteAttributeString(pattern, a.Pattern);

         WriteAnnotation(a.Annotation);

         writer.WriteEndElement();
      }

      void WritePattern(Pattern p)
      {
         writer.WriteStartElement(pattern, namespaceURI);

         if (p.IsAbstract) writer.WriteAttributeString(abstractName, "true");
         if (!string.IsNullOrEmpty(p.BasePatternID)) writer.WriteAttributeString(isa, p.BasePatternID);
         if (!string.IsNullOrEmpty(p.ID)) writer.WriteAttributeString(id, p.ID);
         if (!string.IsNullOrEmpty(p.Fpi)) writer.WriteAttributeString(fpi, p.Fpi);
         if (!string.IsNullOrEmpty(p.Icon)) writer.WriteAttributeString(icon, p.Icon);
         if (!string.IsNullOrEmpty(p.See)) writer.WriteAttributeString(see, p.See);

         if (p.Title.Count > 0)
            WriteAnnotation(p.Title, title);
         WriteAnnotation(p.Annotation);
         if (p.HasParameters)
         {
            foreach (string key in p.Parameters)
            {
               writer.WriteStartElement(param, namespaceURI);
               writer.WriteAttributeString(name, key);
               writer.WriteAttributeString(value, p.Parameters[key]);
               writer.WriteEndElement();
            }
         }
         foreach (Rule rule in p.Rules)
            WriteRule(rule);

         writer.WriteEndElement();
      }

      void WriteRule(Rule r)
      {
         writer.WriteStartElement(rule, namespaceURI);

         if (!string.IsNullOrEmpty(r.Context)) writer.WriteAttributeString(context, r.Context);
         if (!string.IsNullOrEmpty(r.ID)) writer.WriteAttributeString(id, r.ID);
         if (r.IsAbstract) writer.WriteAttributeString(abstractName, "true");
         if (!string.IsNullOrEmpty(r.Flag)) writer.WriteAttributeString(flag, r.Flag);
         if (!string.IsNullOrEmpty(r.Fpi)) writer.WriteAttributeString(fpi, r.Fpi);
         if (!string.IsNullOrEmpty(r.Icon)) writer.WriteAttributeString(icon, r.Icon);
         if (!string.IsNullOrEmpty(r.See)) writer.WriteAttributeString(see, r.See);
         if (!string.IsNullOrEmpty(r.Role)) writer.WriteAttributeString(role, r.Role);
         if (!string.IsNullOrEmpty(r.Subject)) writer.WriteAttributeString(subject, r.Subject);

         if (r.HasParameters)
         {
            foreach (string key in r.Parameters)
            {
               writer.WriteStartElement(let, namespaceURI);
               writer.WriteAttributeString(name, key);
               writer.WriteAttributeString(value, r.Parameters[key]);
               writer.WriteEndElement();
            }
         }
         if (r.HasExtensions)
         {
            foreach (Extends extends in r.Extends)
               WriteExtends(extends);
         }
         foreach (Assertion assertion in r.Assertions)
            WriteAssertion(assertion);

         writer.WriteEndElement();
      }

      void WriteExtends(Extends e)
      {
         writer.WriteStartElement(extends, namespaceURI);

         writer.WriteAttributeString(rule, e.RuleID);

         writer.WriteEndElement();
      }

      void WriteAssertion(Assertion a)
      {
         writer.WriteStartElement(a is Report ? report : assert, namespaceURI);

         if (!string.IsNullOrEmpty(a.Test)) writer.WriteAttributeString(test, a.Test);
         if (!string.IsNullOrEmpty(a.ID)) writer.WriteAttributeString(id, a.ID);
         if (!string.IsNullOrEmpty(a.Diagnostics)) writer.WriteAttributeString(diagnostics, a.Diagnostics);
         if (!string.IsNullOrEmpty(a.Flag)) writer.WriteAttributeString(flag, a.Flag);
         if (!string.IsNullOrEmpty(a.Role)) writer.WriteAttributeString(role, a.Role);
         if (a.Subject != ".") writer.WriteAttributeString(subject, a.Subject);
         if (!string.IsNullOrEmpty(a.Fpi)) writer.WriteAttributeString(fpi, a.Fpi);
         if (!string.IsNullOrEmpty(a.Icon)) writer.WriteAttributeString(icon, a.Icon);
         if (!string.IsNullOrEmpty(a.See)) writer.WriteAttributeString(see, a.See);

         WriteAnnotation(a.Message);
         writer.WriteEndElement();
      }

      void WriteDiagnostic(Diagnostic d)
      {
         writer.WriteStartElement(diagnostic, namespaceURI);

         if (!string.IsNullOrEmpty(d.ID)) writer.WriteAttributeString(id, d.ID);
         if (!string.IsNullOrEmpty(d.Fpi)) writer.WriteAttributeString(fpi, d.Fpi);
         if (!string.IsNullOrEmpty(d.Icon)) writer.WriteAttributeString(icon, d.Icon);
         if (!string.IsNullOrEmpty(d.See)) writer.WriteAttributeString(see, d.See);

         WriteAnnotation(d.Message);

         writer.WriteEndElement();
      }

      void WriteNode(XmlNode node)
      {
         XmlNodeReader reader = new XmlNodeReader(node);
         while (reader.Read())
         {
            WriteNode(reader);
         }
      }

      void WriteNode(XmlReader reader)
      {
         string ns = reader.NamespaceURI;
         if (ns == Schematron.OriginalNamespace)
            ns = Schematron.IsoNamespace;
         switch (reader.NodeType)
         {
            case XmlNodeType.Attribute:
               if (!reader.IsDefault)
                  writer.WriteAttributeString(reader.Prefix, reader.LocalName, ns, reader.Value);
               break;
            case XmlNodeType.CDATA:
               writer.WriteCData(reader.Value);
               break;
            case XmlNodeType.Comment:
               writer.WriteComment(reader.Value);
               break;
            case XmlNodeType.Element:
               writer.WriteStartElement(reader.LocalName, ns);
               if (reader.HasAttributes)
               {
                  reader.MoveToFirstAttribute();
                  do
                  {
                     WriteNode(reader);
                  } while (reader.MoveToNextAttribute());
                  reader.MoveToElement();
               }
               if (reader.IsEmptyElement)
                  writer.WriteEndElement();
               break;
            case XmlNodeType.EndElement:
               writer.WriteEndElement();
               break;
            case XmlNodeType.SignificantWhitespace:
            case XmlNodeType.Whitespace:
               writer.WriteWhitespace(reader.Value);
               break;
            case XmlNodeType.Text:
               writer.WriteString(reader.Value);
               break;
         }

      }
   }
}
