using System;
using System.Collections.Generic;
using System.IdentityModel;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel.Security.Tokens;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Sepia.OpenIdConnect
{
    /// <summary>
    ///   A JSON data structure that represents a cryptographic key.  These keys can be either asymmetric 
    ///   or symmetric.  They can hold both public and private information about the key.
    /// </summary>
    public class JsonWebKey
    {
        /// <summary>
        ///   Indexed by key type ("kty") and contains the the supported algorithms ("alg")
        /// </summary>
        static Dictionary<string, string[]> Algorithms = new Dictionary<string, string[]>
        {
            {"EC", new[] {"ES256", "ES384", "ES512"} }, // Elliptic Curve
            {"RSA", new[] {"RS256", "RS384", "RS512", "PS256", "PS384", "PS512"} }, // RSA
            {"oct", new[] {"HS256", "HS384", "HS512"} }, // Octet sequence (symmetric key)
        };

        /// <summary>
        ///   Creates a new instance of the <see cref="JsonWebKey"/> class from the specified
        ///   JSON 
        /// </summary>
        /// <param name="json">
        ///   See <see href="http://tools.ietf.org/id/draft-ietf-jose-json-web-key-18.html">JSON Web Key (JWK)</see> for details.
        /// </param>
        public JsonWebKey(JObject json)
        {
            Guard.IsNotNull(json, "json");

            Json = json;
            Validate();
        }

        /// <summary>
        ///   The underlaying JSON associated with the class.
        /// </summary>
        public JObject Json { get; private set; }

        /// <summary>
        ///   The cryptographic algorithm family ("kty").
        /// </summary>
        public string KeyType { get { return Json.Value<string>("kty");  } }

        /// <summary>
        ///   Intended use ("use").
        /// </summary>
        /// <value>
        ///   "sig" or "enc".
        /// </value>
        public string Usage { get { return Json.Value<string>("use"); } }

        /// <summary>
        ///  Algorithm intended for use with the key ("alg").
        /// </summary>
        public string Algorithm { get { return Json.Value<string>("alg"); } }

        /// <summary>
        ///  Unique identifier for the key ("kid").
        /// </summary>
        public string Id { get { return Json.Value<string>("kid"); } }

        /// <summary>
        ///   Verify that the JSON is correct.
        /// </summary>
        /// <remarks>
        ///   Throws an <see cref="Exception"/> when invalid.
        /// </remarks>
        public void Validate() // TODO
        {
            string[] algorithms;
            if (Id == null)
                throw new Exception("The key id ('kid') is missing.");
            if (!Algorithms.TryGetValue(KeyType, out algorithms))
                throw new Exception(string.Format("Unknown key type '{0}'.", KeyType));
            if (Algorithm != null && !algorithms.Contains(Algorithm))
                throw new Exception(string.Format("Unknown algorithm '{0}' for key type '{1}'.", Algorithm, KeyType));
            
            // TODO:  A RSA key of size 2048 bits or larger MUST be used with these algorithms.

        }

        /// <summary>
        ///   Converts the JSON Web Key (JWK) into a <see cref="SecurityKey"/>.
        /// </summary>
        public SecurityKey ToSecurityKey()
        {
            switch (KeyType)
            {
                case "RSA":
                    var parameters = new RSAParameters
                    {
                        Modulus = Base64Url.Decode(Json.Value<string>("n")),
                        Exponent = Base64Url.Decode(Json.Value<string>("e")),
                    };
                    var rsa = new RSACryptoServiceProvider();
                    rsa.ImportParameters(parameters);
                    return new RsaSecurityKey(rsa);

                case "oct":
                    return new InMemorySymmetricSecurityKey(Base64Url.Decode(Json.Value<string>("k")));

                case "EC":
                    throw new NotSupportedException("Windows Identity Framework (WIF) does not support Elliptical Curves (ECDSA).");
                
                default:
                    throw new Exception(string.Format("Unknown key type '{0}'.", KeyType));
            }
        }

        /// <summary>
        ///   Converts the JSON Web Key (JWK) into a <see cref="SecurityToken"/>.
        /// </summary>
        public SecurityToken ToSecurityToken()
        {
            switch (KeyType)
            {
                case "RSA":
                    var parameters = new RSAParameters
                    {
                        Modulus = Base64Url.Decode(Json.Value<string>("n")),
                        Exponent = Base64Url.Decode(Json.Value<string>("e")),
                    };
                    var rsa = new RSACryptoServiceProvider();
                    rsa.ImportParameters(parameters);
                    return new RsaSecurityToken(rsa, Id);

                case "oct":
                    return new BinarySecretSecurityToken(Id, Base64Url.Decode(Json.Value<string>("k")));

                case "EC":
                    throw new NotSupportedException("Windows Identity Framework (WIF) does not support Elliptical Curves (ECDSA).");

                default:
                    throw new Exception(string.Format("Unknown key type '{0}'.", KeyType));
            }
        }

    }
}
