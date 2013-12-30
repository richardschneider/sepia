using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Sepia.Schematron.Configuration
{
   /// <summary>
   ///   Implements a configuration section for the <see cref="ProviderFactory{T}"/>.
   /// </summary>
   /// <example>
   /// <code>
   /// &lt;configuration>
   ///    &lt;configSections>
   ///       &lt;section name="<see cref="ProviderFactory{T}.SectionName"/>" type="Sepia.Schematron.Configuration.ProviderSection, Sepia" />
   ///    &lt;/configSections>
   /// 
   ///    &lt;<see cref="ProviderFactory{T}.SectionName"/> defaultProvider="Sample">
   ///          &lt;add name="Sample" type="<i>type name</i>, <i>assembly</i>" />
   ///    &lt;/<see cref="ProviderFactory{T}.SectionName"/>>
   /// &lt;/configuration>
   /// </code>
   /// </example>
   public class ProviderSection : ConfigurationSection
   {
      private ConfigurationProperty defaultProvider = new ConfigurationProperty("default", typeof(string), null);
      private ConfigurationProperty providers = new ConfigurationProperty(null, typeof(ProviderSettingsCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

      /// <summary>
      ///   Gets or sets the name of the default passport provider.
      /// </summary>
      [ConfigurationProperty("default")]
      public string DefaultProvider
      {
         get { return (string)base[defaultProvider]; }
         set { base[defaultProvider] = value; }
      }

      /// <summary>
      ///   Gets the provider settings collection.
      /// </summary>
      [ConfigurationProperty(null, IsDefaultCollection = true)]
      public ProviderSettingsCollection Providers
      {
         get { return (ProviderSettingsCollection)base[providers]; }
      }


   }
}
