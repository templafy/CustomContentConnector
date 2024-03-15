namespace CustomContentConnectorExample.Api;

public static class TokenApi
{
    private static readonly object DummyAccessToken = new
    {
        access_token = Constants.Token.Value,
        token_type = "Bearer",
        expires_in = 3600
    };

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

        if (clientId == Constants.Secrets.ClientId && code == Constants.Code.Value)
        {
            var codeVerifier = bodyForm["code_verifier"];
            var isPkceFlow = !string.IsNullOrEmpty(codeVerifier);
            var isAuthorizationCodeFlow = !string.IsNullOrEmpty(clientSecret) && !isPkceFlow;
            if (isPkceFlow)
            {
                // Validate code_verifier

                return Results.Ok(DummyAccessToken);
            }

            if (isAuthorizationCodeFlow && clientSecret == Constants.Secrets.ClientSecret)
            {
                return Results.Ok(DummyAccessToken);
            }
        }

        return Results.Unauthorized();
    }
}
