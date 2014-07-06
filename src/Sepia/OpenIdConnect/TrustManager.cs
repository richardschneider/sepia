using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Protocols;

namespace Sepia.OpenIdConnect
{
    /// <summary>
    ///   Coordinates the trust relationship between the application and token issuers.
    /// </summary>
    public class TrustManager
    {
        /// <summary>
        ///   Creates a new instance of the <see cref="TrustManager"/> class with the
        ///   default values.
        /// </summary>
        public TrustManager()
        {
            Issuers = new List<IConfigurationManager<OpenIdConnectConfiguration>>();
            TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKeyResolver = SigningKeyResolver,
                ValidIssuers = ValidIssuers()
            };
        }

        /// <summary>
        ///   Configuration details of trusted issuers of tokens. 
        /// </summary>
        public List<IConfigurationManager<OpenIdConnectConfiguration>> Issuers { get; set; }

        /// <summary>
        ///   The parameters used to validate a token.
        /// </summary>
        public TokenValidationParameters TokenValidationParameters { get; private set; }

        IEnumerable<string> ValidIssuers()
        {
            foreach (var issuer in Issuers)
            {
                yield return issuer.GetConfigurationAsync(CancellationToken.None).Result.Issuer;
            }
        }

        SecurityKey SigningKeyResolver(string token, SecurityToken securityToken, SecurityKeyIdentifier keyIdentifier, TokenValidationParameters validationParameters)
        {
            foreach (var keyIdentifierClause in keyIdentifier)
            {
                var match = Issuers
                    .SelectMany(issuer => issuer.GetConfigurationAsync(CancellationToken.None).Result.SigningTokens)
                    .FirstOrDefault(t => t.MatchesKeyIdentifierClause(keyIdentifierClause));
                if (match != null)
                    return match.SecurityKeys[0];
            }
            return null;
        }
    }
}
