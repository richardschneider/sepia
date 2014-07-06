Some types to simplify using [Open ID Connect](http://openid.net/) in a resource server.
  
Use the [`Authentication Server`](AuthenticationServer.cs) to define an authentication server, aka Open ID Provider or Token Issuer, 
that is trusted.  Only the base address of the server is needed.  The end-point `.well-known/openid-configuration`
is used to retrieve a configuration document and from that the signed keys are retrieved.

The [`Trust Manager`](TrustManager.cs) contains the list of all trusted authentication servers and can be
used to validate a security token.

The following states that Google and Microsoft Azure are trusted:

	var trust = new TrustManager
	{
		Issuers = 
		{
			new AuthenticationServer("https://accounts.google.com"),
			new AuthenticationServer("https://login.windows.net/common/")
        }
	};

To verify/validate a token; assuming the token is in the `HTTP Authorization` header:

	var tokenHandler = new JwtSecurityTokenHandler();
	request.GetRequestContext().Principal = tokenHandler.ValidateToken(
		request.Headers.Authorization.Parameter,
		trust.TokenValidationParameters
	);

