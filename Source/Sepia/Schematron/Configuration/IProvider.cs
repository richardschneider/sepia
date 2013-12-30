using System;
using System.Collections.Specialized;
using System.Text;

namespace Sepia.Schematron.Configuration
{
   /// <summary>
   ///   The extensible provider model.
   /// </summary>
   /// <remarks>
   ///   The provider model allows multiple implementations of some feature.  The feature is defined by another interface that
   ///   inherits from the <b>IProvider</b> interface.
   ///  <para>
   ///   A provider is usually created by <see cref="ProviderFactory{T}"/>, which obtains the providers for the feature from the configuration
   ///   settings.
   ///  </para>
   /// </remarks>
   public interface IProvider
   {
      /// <summary>
      ///   Initialises the provider with a collection of configuration settings.
      /// </summary>
      /// <param name="settings">
      ///   A <see cref="NameValueCollection"/> containing the configuration settings for the provider.  This can be <b>null</b>.
      /// </param>
      void Initialize(NameValueCollection settings);

      /// <summary>
      ///   Gets or sets the name of the provider.
      /// </summary>
      string Name { get; set; }

      /// <summary>
      ///   Gets or sets a short description of the provider.
      /// </summary>
      string Description { get; set; }
   }
}
