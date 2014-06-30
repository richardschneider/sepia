using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;

namespace Sepia.OpenIdConnect
{
    /// <summary>
    ///   The metadata describing the <see cref="AuthenticationServer"/>.
    /// </summary>
    public class ConfigurationDocument
    {
        /// <summary>
        ///   The underlaying JSON associated with the document.
        /// </summary>
        public JObject Json { get; set; }

        /// <summary>
        ///   The <see cref="AuthenticationServer.Identifier">issue identifier</see>.
        /// </summary>
        public string Issuer
        {
            get
            {
                return Json.Value<string>("issuer");
            }
        }

        /// <summary>
        ///   The URL of the servers's JSON Web Key Set document.
        /// </summary>
        public Uri KeySetUri
        {
            get
            {
                return new Uri(Json.Value<string>("jwks_uri"));
            }
        }

        /// <summary>
        ///   Verify that the document is correct and issued from the
        ///   specified <see cref="AuthenticationServer"/>.
        /// </summary>
        /// <param name="issuer">
        ///   The <see cref="AuthenticationServer"/> that issued the document.
        /// </param>
        /// <remarks>
        ///   Throws an <see cref="Exception"/> when invalid.
        /// </remarks>
        public void Validate(AuthenticationServer issuer) // TODO
        {
        }

    }
}
