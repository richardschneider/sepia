using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Sepia.OpenIdConnect
{
    /// <summary>
    ///   Units tests for <see cref="NullCache"/>
    /// </summary>
    [TestClass]
    public class AuthenticationServerTest
    {
        /// <summary>
        ///   All server's must have a verifiable identifier.
        /// </summary>
        [TestMethod]
        public void VerifiableIdentifier()
        {
            var server = new AuthenticationServer();

            server.Identifier = new Uri("https://accounts.google.com");
            Assert.AreEqual(new Uri("https://accounts.google.com"), server.Identifier);

            // HTTPS required.
            ExceptionAssert.Throws<ArgumentException>(() => server.Identifier = new Uri("http://accounts.google.com"));

            // Query component not allowed.
            ExceptionAssert.Throws<ArgumentException>(() => server.Identifier = new Uri("https://accounts.google.com?notallowed=true"));

            // Fragment component not allowed.
            ExceptionAssert.Throws<ArgumentException>(() => server.Identifier = new Uri("https://accounts.google.com#notallowed"));
        }

        /// <summary>
        ///   The configuration document can be discovered and the signing keys fetched
        ///   from the server.
        /// </summary>
        [TestMethod]
        public void Discover_Metadata()
        {
            var server = new AuthenticationServer();
            server.Identifier = new Uri("https://accounts.google.com");
            server.DiscoverConfiguration();
            Assert.IsNotNull(server.Configuration, "metadata is missing");
            Assert.AreNotEqual(0, server.SigningTokens.Count(), "signing tokens missing");
        }

        [TestMethod]
        public void ValidateToken_RS256()
        {
            var token = "eyJhbGciOiJSUzI1NiIsImtpZCI6IjUxMDdmYTJmNTM0Y2FlNWFlNGU1MDdmMjEyYzgzMGU0OWU5M2YxNmMifQ.eyJpc3MiOiJhY2NvdW50cy5nb29nbGUuY29tIiwic3ViIjoiMTA5ODI0MjI5NDk0NjA0MTUzNDI5IiwiYXpwIjoiODM0NTQ1Mjk1Mzg0LXBwYTVxZTlham5kNzFvODlwbXNvMDRhbmplc2plNmozLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwiZW1haWwiOiJtYWthcmV0dUBnbWFpbC5jb20iLCJhdF9oYXNoIjoiOU1Va2cwRVZTSVFKRkJaRjJ6dndIQSIsImVtYWlsX3ZlcmlmaWVkIjp0cnVlLCJhdWQiOiI4MzQ1NDUyOTUzODQtcHBhNXFlOWFqbmQ3MW84OXBtc28wNGFuamVzamU2ajMuYXBwcy5nb29nbGV1c2VyY29udGVudC5jb20iLCJpYXQiOjE0MDQyMDMyNzUsImV4cCI6MTQwNDIwNzE3NX0.XbDQKoYMCTXAzT5by056aNn5eeW-mXzl8ANOc9uR_2wFE3WcmghxYRvFW4WDpuxtLZAuom3G1p78t_z2-jxB5J3-J_Q0AHFncnyOU9i3ieZfjAsKxI-0f_PFME03M8uIXoP64nog45S5qzs4POV33waqCV6vmYkLjIwnkInwuXM";
            var jwk = JObject.Parse(@"{
   'kty': 'RSA',
   'alg': 'RS256',
   'use': 'sig',
   'kid': '5107fa2f534cae5ae4e507f212c830e49e93f16c',
   'n': 'ANxMFBIk0JTzgiM5XSloHqQ/dxRP7Kes4jiqBqboAfzSeYmW+RLaaFJnDmkEFbzf/jI3SvRDxKtB6BK9kR/PZn7MXpABERO5/6d8or6U7zcW+uUyWPjxNgLS5F6OEQCsohFpMXeZbsHkyhn2aD+4cKv4gcT24kexT4Btocox5tLJ',
   'e': 'AQAB'
}");
            var server = new AuthenticationServer();
            server.Identifier = new Uri("https://accounts.google.com");
            server.SigningTokens = new[]
            {
                new JsonWebKey(jwk).ToSecurityToken()
            };
            var principle = server.ValidateToken(token);          
        }

        [TestMethod]
        public void ValidateToken_HS256()
        {
            var token = @"eyJ0eXAiOiJKV1QiLA0KICJhbGciOiJIUzI1NiJ9.eyJpc3MiOiJqb2UiLA0KICJleHAiOjEzMDA4MTkzODAsDQogImh0dHA6Ly9leGFtcGxlLmNvbS9pc19yb290Ijp0cnVlfQ.dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk";
            var jwk = JObject.Parse(@"{
  'kty' : 'oct',
  'kid' : '1',
  'k'   : 'AyM1SysPpbyDfgZld3umj1qzKObwVMkoqQ-EstJQLr_T-1qS0gZH75aKtMN3Yj0iPS4hcgUuTwjAzZr1Z9CAow'
     }");
            var server = new AuthenticationServer();
            server.SigningTokens = new[]
            {
                new JsonWebKey(jwk).ToSecurityToken()
            };
            var principle = server.ValidateToken(token);
        }
    }
}
