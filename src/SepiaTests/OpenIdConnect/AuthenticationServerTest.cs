using System;
using System.Linq;
using System.Threading;
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
        ///   Server's that support discovery must have a home address.
        /// </summary>
        [TestMethod]
        public void Home()
        {
            var server = new AuthenticationServer("https://accounts.google.com");

            // HTTPS required.
            ExceptionAssert.Throws<ArgumentException>(() => new AuthenticationServer("http://accounts.google.com"));

            // Query component not allowed.
            ExceptionAssert.Throws<ArgumentException>(() => new AuthenticationServer("https://accounts.google.com?notallowed=true"));

            // Fragment component not allowed.
            ExceptionAssert.Throws<ArgumentException>(() => new AuthenticationServer("https://accounts.google.com#notallowed"));
        }

        /// <summary>
        ///   The configuration document can be discovered and the signing keys fetched
        ///   from the server.
        /// </summary>
        [TestMethod]
        public void Discover_Metadata()
        {
            var server = new AuthenticationServer("https://accounts.google.com");
            var config = server.GetConfigurationAsync(CancellationToken.None).Result;
            Assert.IsNotNull(config, "metadata is missing");
            Assert.AreNotEqual(0, config.SigningTokens.Count(), "signing tokens missing");
        }

    }
}
