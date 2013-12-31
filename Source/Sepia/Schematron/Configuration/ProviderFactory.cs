using Common.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using System.Text;

namespace Sepia.Schematron.Configuration
{
   /// <summary>
   ///   Manages a group of <see cref="IProvider">provider</see>.
   /// </summary>
   /// <typeparam name="Feature">
   ///   The interface for the provider feature.  This feature must implement <see cref="IProvider"/>.
   /// </typeparam>
   /// <remarks>
   ///   <b>ProviderFactory</b> is a class that allows multiple implementations of a <typeparamref name="Feature"/>
   ///   to be specified in a <see cref="ConfigurationManager">configuration</see> <see cref="ProviderSection">section</see>.
   /// </remarks>
   /// <seealso cref="ConfigurationManager"/>
   /// <seeaslo cref="IProvider"/>
   /// <seealso cref="ProviderSection"/>
   public class ProviderFactory<Feature>
      where  Feature : IProvider
   {
      private static ILog log = LogManager.GetLogger("Sepia.Schematron.Configuration.ProviderFactory");

      private Dictionary<string, Feature> providers;
      private string sectionName;
      private Feature defaultProvider;

      /// <summary>
      ///   Creates a new instance of the <see cref="ProviderFactory{Feature}"/> class with the specified
      ///   <see cref="SectionName"/>
      /// </summary>
      /// <param name="sectionName">
      ///   The <see cref="ConfigurationManager">configuration</see> section path and name.
      /// </param>
      public ProviderFactory(string sectionName)
      {
         if (string.IsNullOrEmpty(sectionName))
            throw new ArgumentNullException("sectionName");

         this.sectionName = sectionName;
      }

      /// <summary>
      ///   Gets the <see cref="ConfigurationManager">configuration</see> section path and name.
      /// </summary>
      public string SectionName
      {
         get { return sectionName; }
      }
 
      /// <summary>
      ///   Gets a dictionary of all providers.
      /// </summary>
      /// <value>
      ///   A dictionary that is keyed by the <see cref="IProvider.Name"/>. 
      /// </value>
      public Dictionary<string, Feature> Providers
      {
         get
         {
            if (providers == null)
               LoadProviders();

            return providers;
         }
      }

      /// <summary>
      ///   Gets the default provider for the <typeparamref name="Feature"/>.
      /// </summary>
      /// <value>
      ///   The default <typeparamref name="Feature"/>.
      /// </value>
      public Feature Default
      {
         get
         {
            if (providers == null)
               LoadProviders();

            return defaultProvider;
         }
      }

      /// <summary>
      ///   Forces the <see cref="Providers"/> to be re-initialised.
      /// </summary>
      /// <remarks>
      ///   Derived classes that <c>override</c> this method must always eventually call this method.
      /// </remarks>
      protected virtual void LoadProviders()
      {
         providers = new Dictionary<string, Feature>(StringComparer.InvariantCultureIgnoreCase);
         ProviderSection config = ConfigurationManager.GetSection(SectionName) as ProviderSection;
         if (config == null)
            return;

         foreach (ProviderSettings provider in config.Providers)
         {
            if (log.IsInfoEnabled)
               log.Info(String.Format("Loading {0} '{1}' from '{2}'.", typeof(Feature).Name, provider.Name, provider.Type));

            Feature f = CreateInstance(provider.Type);
            InitialiseProvider(f, provider);
            providers.Add(provider.Name, f);
         }

         if (!string.IsNullOrEmpty(config.DefaultProvider))
         {
            defaultProvider = providers[config.DefaultProvider];
         }
      }

      /// <summary>
      ///   Initialises a new provider.
      /// </summary>
      /// <param name="provider">
      /// </param>
      /// <param name="settings">
      /// </param>
      /// <remarks>
      ///   Sets the <see cref="IProvider.Name"/> and <see cref="IProvider.Description"/> properties of the <paramref name="provider"/> and
      ///   <see cref="IProvider.Initialize">initialises</see> it with the <paramref name="settings"/>.
      ///   Derived classes that <c>override</c> this method must always eventually call this method.
      /// </remarks>
      protected virtual void InitialiseProvider(Feature provider, ProviderSettings settings)
      {
         provider.Name = settings.Name;
         provider.Description = settings.Parameters["description"];
         provider.Initialize(settings.Parameters);
      }

      /// <summary>
      ///   Creates a provider with the given type name.
      /// </summary>
      /// <param name="typeString">
      ///   A comma separated list containing the full name of the type and the assembly to find it.
      /// </param>
      /// <returns>
      ///   A newly created object then implements <typeparamref name="Feature"/>.
      /// </returns>
      protected virtual Feature CreateInstance(string typeString)
      {
         int i = typeString.IndexOf(',');
         Assembly asm;
         string typeName;
         if (i < 0) // No assembly info
         {
            asm = Assembly.GetExecutingAssembly();
            typeName = typeString.Trim();
         }
         else
         {
            typeName = typeString.Substring(0, i).Trim();
            string asmName = typeString.Substring(i + 1).Trim();
            asm = Assembly.Load(asmName);
         }
         Type t = asm.GetType(typeName, true, false);
         return (Feature)Activator.CreateInstance(t);
      }
   }
}
