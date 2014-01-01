using Sepia.Schematron.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sepia.Schematron.Queries
{
    /// <summary>
   ///   Provides schematron query language bindings.
   /// </summary>
   /// <remarks>
   ///   The default set of query languages are specified in the <c>&lt;schematron/queryLanguages></c>
   ///   section of the configuration file.
   ///   <para>
   ///   If not specified, default implementations of "xpath", "xslt" and "exslt" are provided.
   ///   </para>
   /// </remarks>
   /// <example>
   /// <code>
   /// &lt;configuration>
   ///   &lt;configSections>
   ///     &lt;sectionGroup name="schematron">
   ///       &lt;section name="queryLanguages" type="Sepia.Schematron.Configuration.ProviderSection, Sepia" />
   ///     &lt;/sectionGroup>
   ///   &lt;/configSections>
   /// 
   ///   &lt;schematron>
   ///     &lt;queryLanguages>
   ///       &lt;add name="YAQL" type="MyYAQL, My" description="A really cool query language." />
   ///     &lt;/queryLanguages>
   ///   &lt;/schematron>
   /// 
   ///  &lt;/configuration>
   /// </code>
   /// </example>
   /// <seealso cref="Schematron.QueryLanguages"/>
   /// <seealso cref="IQueryLanguage"/>
   public class QueryLanguageFactory : ProviderFactory<IQueryLanguage>
   {
      /// <summary>
      ///   Creates a new instance of the <see cref="QueryLanguageFactory"/> class.
      /// </summary>
      /// <remarks>
      ///   Any <see cref="IQueryLanguage">query languages</see> specified in the <c>&lt;schematron/queryLanguages></c>
      ///   section of the configuration file will be loaded.  If not specified, default implementations of "xpath" and
      ///   "xslt" are provided.
      /// </remarks>
      public QueryLanguageFactory()
         : base("schematron/queryLanguages")
      {
      }

      /// <summary>
      ///   Loads the query languages.
      /// </summary>
      /// <remarks>
      ///   If not specified, default implementations of "xpath" and "xslt" are provided.
      /// </remarks>
      protected override void LoadProviders()
      {
         base.LoadProviders();

         if (!Providers.ContainsKey("xpath"))
            Providers.Add("xpath", new XPathQueryLanguage());
         if (!Providers.ContainsKey("xslt"))
            Providers.Add("xslt", new XsltQueryLanguage());
         if (!Providers.ContainsKey("exslt"))
            Providers.Add("exslt", new ExsltQueryLanguage());
         if (!Providers.ContainsKey("xpath2"))
             Providers.Add("xpath2", new XPath2QueryLanguage());
         if (!Providers.ContainsKey("xslt2"))
             Providers.Add("xslt2", new Xslt2QueryLanguage());
      }
   }
}
