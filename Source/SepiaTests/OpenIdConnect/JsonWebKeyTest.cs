using System;
using System.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace Sepia.OpenIdConnect
{
    /// <summary>
    ///   Units tests for <see cref="NullCache"/>
    /// </summary>
    [TestClass]
    public class JsonWebKeyTest
    {
        /// <summary>
        ///   A JWK can be parsed.
        /// </summary>
        [TestMethod]
        public void ValidKey()
        {
            var jwk = JObject.Parse(
                @"{'kty':'EC',
                'crv':'P-256',
                'x':'MKBCTNIcKUSDii11ySs3526iDZ8AiTo7Tu6KPAqv7D4',
                'y':'4Etl6SRW2YiLUrN5vfvVHuhp7x8PxltmWWlbbM4IFyM',
                'use':'enc',
                'kid':'1'}
                ");
            var key = new JsonWebKey(jwk);
            Assert.AreEqual("1", key.Id);
            Assert.AreEqual("EC", key.KeyType);
            Assert.AreEqual(null, key.Algorithm);
        }

        /// <summary>
        ///   Algorithms for the key are based on the key type.
        /// </summary>
        [TestMethod]
        public void InvalidAlgorithm()
        {
            var jwk = JObject.Parse(
                @"{'kty':'EC',
                'crv':'P-256',
                'x':'MKBCTNIcKUSDii11ySs3526iDZ8AiTo7Tu6KPAqv7D4',
                'y':'4Etl6SRW2YiLUrN5vfvVHuhp7x8PxltmWWlbbM4IFyM',
                'use':'enc',
                'alg':'RS256',
                'kid':'1'}
                ");
            ExceptionAssert.Throws(() =>
            {
                var key = new JsonWebKey(jwk);
            });
        }

        /// <summary>
        ///   All keys must have a key ID.
        /// </summary>
        [TestMethod]
        public void KeyIdIsRequired()
        {
            var jwk = JObject.Parse(
                @"{'kty':'EC',
                'crv':'P-256',
                'x':'MKBCTNIcKUSDii11ySs3526iDZ8AiTo7Tu6KPAqv7D4',
                'y':'4Etl6SRW2YiLUrN5vfvVHuhp7x8PxltmWWlbbM4IFyM',
                'use':'enc'}
                ");
            ExceptionAssert.Throws(() =>
            {
                var key = new JsonWebKey(jwk);
            });
        }

        /// <summary>
        ///   An RSA public key can be a security key.
        /// </summary>
        [TestMethod]
        public void SecurityKey_Rsa()
        {
            var jwk = JObject.Parse(
                @"{'kty':'RSA',
                   'n': '0vx7agoebGcQSuuPiLJXZptN9nndrQmbXEps2aiAFbWhM78LhWx4cbbfAAtVT86zwu1RK7aPFFxuhDR1L6tSoc_BJECPebWKRXjBZCiFV4n3oknjhMstn64tZ_2W-5JsGY4Hc5n9yBXArwl93lqt7_RN5w6Cf0h4QyQ5v-65YGjQR0_FDW2QvzqY368QQMicAtaSqzs8KJZgnYb9c7d0zgdAZHzu6qMQvRL5hajrn1n91CbOpbISD08qNLyrdkt-bFTWhAI4vMQFh6WeZu0fM4lFd2NcRwr3XPksINHaQ-G_xBniIqbw0Ls1jF44-csFCur-kEgU8awapJzKnqDKgw',
                   'e':'AQAB',
                   'alg':'RS256',
                   'kid':'2011-04-29'}
                ");
            var securityKey = new JsonWebKey(jwk).ToSecurityKey();
            Assert.IsNotNull(securityKey);
            Assert.IsInstanceOfType(securityKey, typeof(RsaSecurityKey));
            Assert.AreEqual(256 * 8, securityKey.KeySize);
        }

        /// <summary>
        ///   An Elliptical Curve public key should be a security key.
        ///   However, WIF doesn't yet support ECDSA.
        /// </summary>
        [TestMethod]
        public void SecurityKey_Ecdsa()
        {
            var jwk = JObject.Parse(
                @"{'kty':'EC',
                'crv':'P-256',
                'x':'MKBCTNIcKUSDii11ySs3526iDZ8AiTo7Tu6KPAqv7D4',
                'y':'4Etl6SRW2YiLUrN5vfvVHuhp7x8PxltmWWlbbM4IFyM',
                'use':'enc',
                'kid':'1'}
                ");
            ExceptionAssert.Throws<NotSupportedException>(() =>
            {
                var key = new JsonWebKey(jwk).ToSecurityKey();
            });
        }

        /// <summary>
        ///   An secret key can be a security key.
        /// </summary>
        [TestMethod]
        public void SecurityKey_Secret()
        {
            var jwk = JObject.Parse(
                @"{
                    'kid':'mysecret',
                    'kty':'oct',
                    'k':'GawgguFyGrWKav7AX4VKUg'
                  }
                ");
            var securityKey = new JsonWebKey(jwk).ToSecurityKey();
            Assert.IsNotNull(securityKey);
            Assert.IsInstanceOfType(securityKey, typeof(SymmetricSecurityKey));
        }

    }
}
