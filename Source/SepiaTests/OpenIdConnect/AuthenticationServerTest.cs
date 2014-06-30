using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        ///   The configuration document can be discovered.
        /// </summary>
        [TestMethod]
        public void Discover_Metadata()
        {
            var server = new AuthenticationServer();
            server.Identifier = new Uri("https://accounts.google.com");
            server.DiscoverConfiguration();
            Assert.IsNotNull(server.Configuration, "metadata is missing");
        }
    }
}
