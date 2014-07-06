using System;
using System.IdentityModel.Tokens;
using System.Linq;
using System.Threading;
using Microsoft.IdentityModel.Protocols;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Sepia.OpenIdConnect
{
    /// <summary>
    ///   Units tests for <see cref="TrustManager"/>
    /// </summary>
    [TestClass]
    public class TrustManagerTest
    {
        /// <summary>
        ///   Open ID Connect Discovery is supported.
        /// </summary>
        [TestMethod]
        public void Discovery()
        {
            var trust = new TrustManager
            {
                Issuers = 
                {
                    new AuthenticationServer("https://accounts.google.com")
                }
            };
            var google = trust.Issuers[0];
            var config = google.GetConfigurationAsync(CancellationToken.None).Result;
            Assert.IsNotNull(config, "metadata is missing");
            Assert.AreNotEqual(0, config.SigningTokens.Count(), "signing tokens missing");
        }

        /// <summary>
        ///   All issuer's have a name.
        /// </summary>
        [TestMethod]
        public void IssuerName()
        {
            var trust = new TrustManager
            {
                Issuers = 
                {
                    new ConfigurationManager<OpenIdConnectConfiguration>("https://accounts.google.com/.well-known/openid-configuration")
                }
            };
            var issuers = trust.TokenValidationParameters.ValidIssuers.ToArray();
            Assert.AreEqual(1, issuers.Length);
            Assert.AreEqual("accounts.google.com", issuers[0]);
        }

        /// <summary>
        ///   Issuers can be added at anytime.
        /// </summary>
        [TestMethod]
        public void AddIssuer()
        {
            var trust = new TrustManager
            {
                Issuers = 
                {
                    new AuthenticationServer("https://accounts.google.com")
                }
            };
            var issuers = trust.TokenValidationParameters.ValidIssuers.ToArray();
            Assert.AreEqual(1, issuers.Length);
            Assert.AreEqual("accounts.google.com", issuers[0]);

            trust.Issuers.Add(new AuthenticationServer("https://login.windows.net/common/"));
            issuers = trust.TokenValidationParameters.ValidIssuers.ToArray();
            Assert.AreEqual(2, issuers.Length);
            Assert.AreEqual("accounts.google.com", issuers[0]);
            Assert.AreEqual("https://sts.windows.net/{tenantid}/", issuers[1]);
        }

        /// <summary>
        ///   The signing key can be found for an issued token.
        /// </summary>
        [TestMethod]
        public void ResolveSigningKey()
        {
            var trust = new TrustManager
            {
                Issuers = 
                {
                    new ConfigurationManager<OpenIdConnectConfiguration>("https://accounts.google.com/.well-known/openid-configuration")
                }
            };
            var google = trust.Issuers[0];
            var config = google.GetConfigurationAsync(CancellationToken.None).Result;
            var signingToken = config.SigningTokens.Last();
            Assert.IsNotNull(signingToken.Id);
            var ski = new SecurityKeyIdentifier
            {
                new NamedKeySecurityKeyIdentifierClause(JwtHeaderParameterNames.Kid, signingToken.Id)
            };
            var signingKey = trust.TokenValidationParameters.IssuerSigningKeyResolver(null, null, ski, null);
            Assert.IsNotNull(signingKey);
            Assert.AreEqual(signingToken.SecurityKeys.Last(), signingKey);
        }
    }
}
