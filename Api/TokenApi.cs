namespace CustomContentConnectorExample.Api;

public static class TokenApi
{
    private static readonly object DummyAccessToken = new
    {
        access_token = Constants.Token.Value,
        token_type = "Bearer",
        expires_in = 3600
    };
    
    /// <summary>
    /// <para>This endpoint will be called by Templafy regardless of the Authentication method selected, in order to retrieve the access token, which Templafy will send
    /// with each subsequent request.</para>
    /// <para>The Authentication method is configured in the Templafy admin portal in the Integrations tab. This can be one the following
    /// <list type="bullet">
    /// <item><description>OAuth 2 Client Credentials</description></item>
    /// <item><description>OAuth 2 Authorization Code</description></item>
    /// <item><description>OAuth 2 Authorization Code with PKCE</description></item>
    /// </list>
    /// </para>
    /// <para>In order to distinguish between OAuth 2 Client Credentials and OAuth 2 Authorization code or OAuth 2 Authorization code with PKCE
    /// the <b>"grant_type</b> request body parameter should be used.</para>
    /// <para>To further distinguish between OAuth 2 Authorization code
    /// and OAuth 2 Authorization code with PKCE the existence of the <b>code_verifier</b> request body parameter should be used. The existence of this parameter
    /// means the OAuth 2 Authorization code with PKCE is configured.</para>
    /// </summary>
    /// <param name="request">The body parameters which are sent by Templafy with this requests are visible bellow when destructuring the <see cref="IFormCollection"/>.</param>
    /// <returns></returns>
    public static IResult HandleTokenRequest(HttpRequest request)
    {
        var bodyForm = request.Form;
        var grantType = bodyForm["grant_type"].ToString();

        return grantType switch
        {
            "client_credentials" => HandleClientCredentials(bodyForm),
            "authorization_code" => HandleAuthorizationCode(bodyForm),
            _ => Results.BadRequest()
        };
    }

    private static IResult HandleClientCredentials(IFormCollection bodyForm)
    {
        var clientId = bodyForm["client_id"];
        var clientSecret = bodyForm["client_secret"];

        if (clientId == Constants.Secrets.ClientId && clientSecret == Constants.Secrets.ClientSecret)
        {
            return Results.Ok(DummyAccessToken);
        }

        return Results.Unauthorized();
    }

    private static IResult HandleAuthorizationCode(IFormCollection bodyForm)
    {
        var clientId = bodyForm["client_id"];
        var clientSecret = bodyForm["client_secret"];
        var code = bodyForm["code"];
        var redirectUri = bodyForm["redirect_uri"];

        if (clientId == Constants.Secrets.ClientId && clientSecret == Constants.Secrets.ClientSecret &&
            code == Constants.Code.Value)
        {
            var codeVerifier = bodyForm["code_verifier"];
            var isPkceFlow = !string.IsNullOrEmpty(codeVerifier);
            if (isPkceFlow)
            {
                // Validate code_verifier

                return Results.Ok(DummyAccessToken);
            }

            return Results.Ok(DummyAccessToken);
        }

        return Results.Unauthorized();
    }
}