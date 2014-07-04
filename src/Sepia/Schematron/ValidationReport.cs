using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Sepia.Schematron
{
   /// <summary>
   /// The Schematron Validation Report Language (SVRL) is a simple XML language used for reporting the
   /// results of the <see cref="SchematronValidator"/>.
   /// </summary>
   /// <remarks>
   ///   <b>ValidationReport</b> monitors the execution of a <see cref="SchematronValidator"/> and records
   ///   the results as an XML document
   /// </remarks>
   /// <example>
   ///   The follow validates the "myinstance.xml" <see cref="XmlDocument"/> against the "my.sch" <see cref="SchematronDocument"/>
   ///   and sends the SVRL to the <see cref="Console"/>.
   /// <code>
   ///   SchematronValidator validator = new SchematronValidator("my.sch");
   ///   ValidationReport report = new ValidationReport(validator, Console.Out);
   ///   XmlDocument instance = XmlReader.Create("myinstance.xml");
   ///   validator.Validate(instance);
   /// </code>
   /// </example>
   public class ValidationReport
   {
      static readonly XmlWriterSettings defaultWriterSettings = new XmlWriterSettings { Indent = true };

      SchematronValidator validator;
      XmlWriter report;
      bool closeStream;
      bool validationFailure;

      /// <summary>
      ///   Creates a new instance of the <see cref="ValidationReport"/> class using the specified <see cref="Stream"/>.
      /// </summary>
      /// <param name="validator">
      ///   The <see cref="SchematronValidator"/> to report.
      /// </param>
      /// <param name="stream">
      ///   The <see cref="Stream"/> used to write the report.
      /// </param>
      public ValidationReport(SchematronValidator validator, Stream stream)
         : this(validator, XmlWriter.Create(stream, defaultWriterSettings))
      {
      }

      /// <summary>
      ///   Creates a new instance of the <see cref="ValidationReport"/> class using the specified <see cref="StringBuilder"/>.
      /// </summary>
      /// <param name="validator">
      ///   The <see cref="SchematronValidator"/> to report.
      /// </param>
      /// <param name="s">
      ///   The <see cref="StringBuilder"/> used to write the report.
      /// </param>
      public ValidationReport(SchematronValidator validator, StringBuilder s)
         : this(validator, XmlWriter.Create(s, defaultWriterSettings))
      {
      }

      /// <summary>
      ///   Creates a new instance of the <see cref="ValidationReport"/> class using the specified filename.
      /// </summary>
      /// <param name="validator"></param>
      /// <param name="filename">
      ///   The name of file to write the report to.
      /// </param>
      public ValidationReport(SchematronValidator validator, string filename)
         : this(validator, XmlWriter.Create(filename, defaultWriterSettings))
      {
         closeStream = true;
      }

      /// <summary>
      ///   Creates a new instance of the <see cref="ValidationReport"/> class using the specified <see cref="TextWriter"/>.
      /// </summary>
      /// <param name="validator"></param>
      /// <param name="writer">
      ///   The <see cref="TextWriter"/> used to write the report.
      /// </param>
      public ValidationReport(SchematronValidator validator, TextWriter writer)
         : this(validator, XmlWriter.Create(writer, defaultWriterSettings))
      {
      }

      /// <summary>
      ///   Creates a new instance of the <see cref="ValidationReport"/> class using the specified <see cref="XmlWriter"/>.
      /// </summary>
      /// <param name="validator"></param>
      /// <param name="report">
      ///   The <see cref="XmlWriter"/> used to write the report.
      /// </param>
      public ValidationReport(SchematronValidator validator, XmlWriter report)
      {
         if (validator == null)
            throw new ArgumentNullException("validator");
         if (report == null)
            throw new ArgumentNullException("report");

         this.validator = validator;
         this.report = report;

         // Hookup the events.
         validator.RuleFired += RuleFired;
         validator.ActivePattern += ActivePattern;
         validator.AssertionFailed += AssertionFailed;
         validator.Start += Start;
         validator.End += End;
      }

      /// <summary>
      ///   Determines if a validation failure has occurred.
      /// </summary>
      /// <value>
      ///   <b>true</b> if the associated <see cref="SchematronValidator"/> has detected
      ///   an validate failure; <b>otherwise</b>, true;
      /// </value>
      public bool HasValidationErrors
      {
         get { return validationFailure; }
      }

      void RuleFired(object sender, SchematronValidationEventArgs e)
      {
         report.WriteStartElement("fired-rule", Schematron.SvrlNamespace);
         if (!string.IsNullOrEmpty(e.Rule.ID)) report.WriteAttributeString("id", e.Rule.ID);
         report.WriteAttributeString("context", e.Rule.Context);
         if (!string.IsNullOrEmpty(e.Rule.Role)) report.WriteAttributeString("role", e.Rule.Role);
         if (!string.IsNullOrEmpty(e.Rule.Flag)) report.WriteAttributeString("flag", e.Rule.Flag);
         report.WriteEndElement();
      }

      void AssertionFailed(object sender, SchematronValidationEventArgs e)
      {
         validationFailure = true;
         if (e.Assertion is Report)
            report.WriteStartElement("successful-report", Schematron.SvrlNamespace);
         else
            report.WriteStartElement("failed-assert", Schematron.SvrlNamespace);
         report.WriteAttributeString("location", XPathHelper.FullName(e.Instance));
         report.WriteAttributeString("test", e.Assertion.Test);
         if (!string.IsNullOrEmpty(e.Assertion.Role)) report.WriteAttributeString("role", e.Assertion.Role);
         if (!string.IsNullOrEmpty(e.Assertion.Flag)) report.WriteAttributeString("flag", e.Assertion.Flag);
         // Diagnostic references
         if (!string.IsNullOrEmpty(e.Assertion.Diagnostics))
         {
            string[] ids = e.Assertion.Diagnostics.Split(' ');
            for (int i = 0; i < ids.Length; ++i)
            {
               report.WriteStartElement("diagnostic-reference", Schematron.SvrlNamespace);
               report.WriteAttributeString("diagnostic", ids[i]);
               if (i < e.Diagnostics.Length)
                  report.WriteElementString("text", Schematron.SvrlNamespace, e.Diagnostics[i]);
               report.WriteEndElement();
            }
         }
         report.WriteElementString("text", Schematron.SvrlNamespace, e.Message);
         report.WriteEndElement();
      }

      void ActivePattern(object sender, SchematronValidationEventArgs e)
      {
         report.WriteStartElement("active-pattern", Schematron.SvrlNamespace);
         if (!string.IsNullOrEmpty(e.Pattern.ID)) report.WriteAttributeString("id", e.Pattern.ID);
         report.WriteAttributeString("name", e.Pattern.Title.ToString());
         report.WriteEndElement();
      }

      void Start(object sender, SchematronValidationEventArgs e)
      {
         report.WriteStartElement("schematron-output", Schematron.SvrlNamespace);
         if (e.Schematron.Title.Count > 0) report.WriteAttributeString("title", e.Schematron.Title.ToString());
         if (!string.IsNullOrEmpty(e.Schematron.SchemaVersion)) report.WriteAttributeString("schemaVersion", e.Schematron.SchemaVersion);
         report.WriteAttributeString("phase", ((SchematronValidator)sender).ValidationPhase);

         if (e.Schematron.HasNamespaces)
         {
            foreach (NamespaceDefinition ns in e.Schematron.Namespaces)
            {
               report.WriteStartElement("ns-prefix-in-attribute-values", Schematron.SvrlNamespace);
               report.WriteAttributeString("prefix", ns.Prefix);
               report.WriteAttributeString("uri", ns.Uri);
               report.WriteEndElement();
            }
         }
      }

      void End(object sender, SchematronValidationEventArgs e)
      {
         report.WriteEndElement();

         report.Flush();
         if (closeStream)
            report.Close();
      }
   }
}
