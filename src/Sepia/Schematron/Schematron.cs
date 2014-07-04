using Sepia.Schematron.Queries;
using Common.Logging;
using System;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Sepia.Schematron
{
   /// <summary>
   ///   The XML schemas of Schematron
   /// </summary>
   /// <remarks>
   ///   <b>Schematron</b> defines the configuration and formatting information of a Schematron assertion document.  It uses the singleton pattern and is accessed via
   ///   the <see cref="Default"/> static property.  All Schematron schemas are stored as resources in the <see cref="Assembly"/>.
   /// </remarks>
   public sealed class Schematron
   {
      private static ILog log = LogManager.GetLogger(typeof(Schematron));
      private XmlSchemaSet set;
      private SchematronDocument isoSchematronSchema;
      private SchematronDocument originalSchematronSchema;
      private SchematronDocument validationReportLanguage;
      private QueryLanguageFactory queryLanguages = new QueryLanguageFactory();

      /// <summary>
      ///   The default <see cref="Sepia.Schematron.Queries.IQueryLanguage">query language</see> is "xslt".
      /// </summary>
      /// <seealso cref="SchematronDocument.QueryLanguage"/>
      /// <seealso cref="Sepia.Schematron.Queries.QueryLanguageFactory"/>
      public const string DefaultQueryLanguage = "xslt";

      /// <summary>
      ///   The namespace uri for an ISO/IEC 19757-3 <see cref="SchematronDocument"/>.
      /// </summary>
      public const string IsoNamespace = "http://purl.oclc.org/dsdl/schematron";

      /// <summary>
      ///   The namespace uri for an ISO/IEC 19757-3 <see cref="ValidationReport">Schematron Validation Report Language</see>.
      /// </summary>
      public const string SvrlNamespace = "http://purl.oclc.org/dsdl/svrl";

      /// <summary>
      ///   The original namespace uri for a <see cref="SchematronDocument"/>.
      /// </summary>
      public const string OriginalNamespace = "http://www.ascc.net/xml/schematron";

      private Schematron()
      {
      }

      /// <summary>
      ///   Gets the BlackHen implementation of Schematron.
      /// </summary>
      /// <value>
      ///   A <see cref="Schematron"/> configuration.
      /// </value>
      public static Schematron Default = new Schematron();

      /// <summary>
      ///   Gets the factory that provides the schematron query language bindings.
      /// </summary>
      /// <value>
      ///   The <see cref="QueryLanguageFactory"/> that provides the <see cref="IQueryLanguage">query language</see> binding.
      /// </value>
      /// <see cref="IQueryLanguage"/>
      /// <remarks>
      ///   The default set of query languages is obtained from the <c>&lt;schematron/queryLanguages></c> configuration section;
      ///   see <see cref="QueryLanguageFactory"/> for more details.
      /// </remarks>
      public QueryLanguageFactory QueryLanguages
      {
         get { return queryLanguages; }
      }

      /// <summary>
      ///   Gets the <see cref="SchematronDocument"/> that validates an ISO/IEC Schematron document.
      /// </summary>
      public SchematronDocument IsoSchematronSchema
      {
         get
         {
            if (isoSchematronSchema == null)
               isoSchematronSchema = LoadSchematron("Sepia.Schematron.Schemas.Schematron-ISO-2006(E).sch", false);

            return isoSchematronSchema;
         }
      }

      /// <summary>
      ///   Gets the <see cref="SchematronDocument"/> that validates an original Schematron document.
      /// </summary>
      public SchematronDocument OriginalSchematronSchema
      {
         get
         {
            if (originalSchematronSchema == null)
               originalSchematronSchema = LoadSchematron("Sepia.Schematron.Schemas.Schematron-1.5.sch", false);

            return originalSchematronSchema;
         }
      }

      /// <summary>
      ///   Gets the <see cref="SchematronDocument"/> that defines the Schematron Validation Report Language.
      /// </summary>
      public SchematronDocument ValidationReportLanguage
      {
         get
         {
            if (validationReportLanguage == null)
               validationReportLanguage = LoadSchematron("Sepia.Schematron.Schemas.Svrl-ISO-2006(E).sch", true);

            return validationReportLanguage;
         }
      }

      /// <summary>
      ///   Gets the W3C XSD schemas that define a <see cref="SchematronDocument"/>.
      /// </summary>
      /// <value>
      ///   A <see cref="XmlSchemaSet"/> containing all the Schematron schemas.
      /// </value>
      public XmlSchemaSet XsdSet
      {
         get 
         {
            if (set == null)
               set = Load();

            return set; 
         }
      }

      private XmlSchemaSet Load()
      {
         XmlSchemaSet schemas = new XmlSchemaSet();
         Assembly asm = Assembly.GetExecutingAssembly();
         string prefix = "Sepia.Schematron.Schemas.";

         foreach (string name in asm.GetManifestResourceNames())
         {
            if (name.StartsWith(prefix) && name.EndsWith(".xsd"))
            {
               if (log.IsInfoEnabled)
                  log.Info("Loading " + name);

               Stream s = asm.GetManifestResourceStream(name);
               if (s == null)
                  throw new Exception(String.Format("The resource '{0}' is missing.", name));

               using (StreamReader reader = new StreamReader(s))
               {
                  schemas.Add(XmlSchema.Read(reader, null));
               }
            }
         }

         return schemas;
      }

      SchematronDocument LoadSchematron(string name, bool schematronValidation)
      {
         if (log.IsInfoEnabled)
            log.Info("Loading " + name);

         XmlDocument xml = new XmlDocument();
         Assembly asm = Assembly.GetExecutingAssembly();
         using (Stream s = asm.GetManifestResourceStream(name))
         {
            if (s == null)
               throw new Exception(String.Format("The resource '{0}' is missing.", name));

           SchematronDocument doc = new SchematronDocument();
           doc.Load(s, schematronValidation);
           return doc;
         }
      }
   }

   
}
