using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Common.Logging;
using Microsoft.IdentityModel.Protocols;

namespace Sepia.OpenIdConnect
{
    /// <summary>
    ///   OAuth 2.0 Authorization Server that is capable of Authenticating the End-User and providing Claims to a 
    ///   Relying Party about the Authentication event and the End-User.
    /// </summary>
    /// <remarks>
    ///   Also referred to as an OpenID Provider (OP).
    /// </remarks>
    public class AuthenticationServer : ConfigurationManager<OpenIdConnectConfiguration>
    {
        static ILog log = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///   Creates a new instance of the <see cref="AuthenticationServer"/> class with the default values.
        /// </summary>
        /// <param name="baseAddress">
        ///   The base URL of the Open ID Connect server, such as "https://accounts.google.com".
        ///   The URL is case sensitive <see cref="Uri"/> using the https scheme that contains scheme, host, 
        ///   and optionally, port number and path components and no query or fragment components.
        /// </param>
        /// <remarks>
        ///   The <see cref="Uri.Scheme"/> must be "https" so that the server's identity can be
        ///   verified.  This prevents a man-in-the-middle attack.
        /// </remarks>
        public AuthenticationServer(string baseAddress)
            : base(DiscoveryUrl(baseAddress), DiscoveryClient())
        {
            AutomaticRefreshInterval = TimeSpan.FromHours(12);
        }

        static string DiscoveryUrl(string baseAddress)
        {
            var uri = new Uri(baseAddress);
            Guard.Require(uri.IsAbsoluteUri, "identifier", "Must be an absolute URL.");
            Guard.Require(uri.Scheme == "https", "identifier", "The 'https' scheme must be used.");
            Guard.Require(string.IsNullOrEmpty(uri.Query), "identifier", "Query component is not allowed.");
            Guard.Require(string.IsNullOrEmpty(uri.Fragment), "identifier", "Fragment component is not allowed.");

            return new Uri(uri, ".well-known/openid-configuration").ToString();
        }

        static HttpClient DiscoveryClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            return new HttpClient(new LoggingHandler(handler));
        }


        sealed class LoggingHandler : DelegatingHandler
        {
            public LoggingHandler(HttpMessageHandler innerHandler)
            {
                Guard.IsNotNull(innerHandler, "innerHnadler");

                InnerHandler = innerHandler;
            }

            /// <inheritdoc />
            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                if (log.IsDebugEnabled)
                    log.DebugFormat("{0} {1}", request.Method, request.RequestUri);

                var response = await base.SendAsync(request, cancellationToken);
                if (log.IsDebugEnabled)
                    log.DebugFormat("Status {0} to {1}", response.StatusCode, response.RequestMessage.RequestUri);

                return response;
            }

        }
    }
    
}
