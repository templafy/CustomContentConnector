namespace CustomContentConnectorExample.Api;

public static class UserApi
{
    /// <summary>
    /// <para>This endpoint will be called once the user presses the "Login" button in the form rendered by the <see cref="AuthorizationApi.RenderAuthorizationForm"/>
    /// during the OAuth2 Authorization code or PKCE flows</para>
    ///
    /// <para>This part should be implemented by the implementer of the API in a secure manner.</para>
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static IResult HandleUserLogin(HttpRequest request)
    {
        var bodyForm = request.Form;
        var username = bodyForm["username"];
        var password = bodyForm["password"];
        var responseType = bodyForm["response_type"];
        var clientId = bodyForm["client_id"];
        var templafyTenantId = bodyForm["state"];
        var redirectUri = bodyForm["redirect_uri"];

        if (responseType == "code" && UserIsCorrect(username, password) && clientId == Constants.Secrets.ClientId)
        {
            var fullRedirectUri = redirectUri + $"?code={Constants.Code.Value}&state={templafyTenantId}";
            return Results.Redirect(fullRedirectUri);
        }

        return Results.Unauthorized();
    }

    private static bool UserIsCorrect(string? username, string? password)
    {
        return username == Constants.User.Username && password == Constants.User.Password;
    }
}