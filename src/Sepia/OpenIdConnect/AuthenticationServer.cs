using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using Common.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sepia.OpenIdConnect
{
    /// <summary>
    ///   OAuth 2.0 Authorization Server that is capable of Authenticating the End-User and providing Claims to a 
    ///   Relying Party about the Authentication event and the End-User.
    /// </summary>
    /// <remarks>
    ///   Also referred to as an OpenID Provider (OP).
    /// </remarks>
    public class AuthenticationServer
    {
        static ILog log = LogManager.GetCurrentClassLogger();

        Uri identifier;
        HttpClient client;
        TokenValidationParameters tokenValidationParameters;

        /// <summary>
        ///   Creates a new instance of the <see cref="AuthenticationServer"/> class with the default values.
        /// </summary>
        public AuthenticationServer()
        {
            ConfigurationPath = "/.well-known/openid-configuration";
            tokenValidationParameters = new TokenValidationParameters();
            client = new HttpClient();
            // TODO: Allow compression.
        }

        /// <summary>
        ///   Verifiable identifier for an <b>AuthenticateServer</b>.
        /// </summary>
        /// <value>
        ///    A case sensitive <see cref="Uri"/> using the https scheme that contains scheme, host, 
        ///    and optionally, port number and path components and no query or fragment components.
        /// </value>
        public Uri Identifier 
        {
            get
            {
                return identifier;
            }
            set
            {
                Guard.IsNotNull(value, "identifier");
                Guard.Require(value.IsAbsoluteUri, "identifier", "Must be an absolute URL.");
                Guard.Require(value.Scheme == "https", "identifier", "The 'https' scheme must be used.");
                Guard.Require(string.IsNullOrEmpty(value.Query), "identifier", "Query component is not allowed.");
                Guard.Require(string.IsNullOrEmpty(value.Fragment), "identifier", "Fragment component is not allowed.");

                identifier = value;
            }
        }

        /// <summary>
        ///   Signing tokens used by the server.
        /// </summary>
        public IEnumerable<SecurityToken> SigningTokens
        {
            get { return tokenValidationParameters.SigningTokens;  }
            set { tokenValidationParameters.SigningTokens = value; }
        }

        /// <summary>
        ///   The path to the configuration document.
        /// </summary>
        /// <value>
        ///   The default value is "/.well-known/openid-configuration".
        /// </value>
        public string ConfigurationPath { get; set; }

        /// <summary>
        ///   Get the metadata associated with the server.
        /// </summary>
        /// <remarks>
        ///   Use the <see cref="DiscoverConfiguration"/> method to initially populate
        ///   the document.
        /// </remarks>
        public ConfigurationDocument Configuration { get; set; }

        /// <summary>
        ///   Retrieve the metadata from the server. 
        /// </summary>
        /// <remarks>
        ///   OpenID servers provide a <see cref="Configuration"/> metadata at the <see cref="ConfigurationPath"/>.
        ///   This can be used to get the <see cref="SigningTokens"/> from the server.
        /// </remarks>
        public void DiscoverConfiguration()
        {
            Configuration = new ConfigurationDocument
            {
                Json = GetDocument(new Uri(Identifier, ConfigurationPath))
            };
            Configuration.Validate(this);

            // Get the JWS keys.
            SigningTokens = GetDocument(Configuration.KeySetUri)["keys"]
                .Cast<JObject>()
                .Select(k => new JsonWebKey(k).ToSecurityToken())
                .ToArray();
        }

        /// <summary>
        ///   TODO
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public IPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.ValidateToken(token, tokenValidationParameters);
        }

        JObject GetDocument(Uri endpoint)
        {
            Guard.IsNotNull(endpoint, "endpoint");
            if (log.IsDebugEnabled)
                log.Debug("GET " + endpoint.ToString());
            Guard.Require(endpoint.IsAbsoluteUri, "endpoint", "Must be an absolute URL.");
            Guard.Require(endpoint.Scheme == "https", "endpoint", "The 'https' scheme must be used.");

            using (Stream s = client.GetStreamAsync(endpoint).Result)
            using (StreamReader sr = new StreamReader(s))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                var serializer = new JsonSerializer();
                var json = serializer.Deserialize(reader);
                return (JObject)json;
            }
        }
    }
}
