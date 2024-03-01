using CustomContentConnectorExample.Utilities;

namespace CustomContentConnectorExample.Api;

public static class AuthorizationApi
{
    /// <summary>
    /// <para><b>This endpoint is only needed for Authorization Code or Authorization Code with PKCE.</b></para>
    ///
    /// <para>The purposes of this endpoint is to render an authentication form for the user to enter their credentials.</para>
    ///
    /// <para>The content of this login form will be rendered by Templafy in a pop-up window once the users pressed the "Log in" button.</para>
    ///
    /// <para>The hidden fields are added as a convenience for the sake of this implementation as a means to be leveraged
    /// in <see cref="UserApi.HandleUserLogin"/> (which is the handler of the endpoint "/login" called once the "Login" button is pressed) and by no means
    /// represent industry standards or security guidelines.</para>
    /// </summary>
    /// <param name="request">The URL query parameters Templafy is sending for this request are deconstructed on lines 24-29</param>
    /// <returns></returns>
    public static IResult RenderAuthorizationForm(HttpRequest request)
    {
        var query = request.Query;

        var responseType = query["response_type"];
        var clientId = query["client_id"];
        var state = query["state"];
        var redirectUri = query["redirect_uri"];
        var codeChallenge = query["code_challenge"];
        var codeChallengeMethod = query["code_challenge_method"];

        var isPkceFlow = !string.IsNullOrEmpty(codeChallenge) && !string.IsNullOrEmpty(codeChallengeMethod);

        var formInputs = @$"
            <input type=""text"" name=""username"" placeholder=""user""/>
            <input type=""password"" name=""password"" placeholder=""password""/>
            <input type=""hidden"" name=""response_type"" value=""{responseType}"" />
            <input type=""hidden"" name=""client_id"" value=""{clientId}"" />
            <input type=""hidden"" name=""state"" value=""{state}"" />
            <input type=""hidden"" name=""redirect_uri"" value=""{redirectUri}"" />
            <input type=""submit"" value=""Login"" />";

        if (isPkceFlow)
        {
            formInputs += @$"
                <input type=""hidden"" name=""code_challenge"" value=""{codeChallenge}"" />
                <input type=""hidden"" name=""code_challenge_method"" value=""{codeChallengeMethod}"" />";
        }

        return Results.Extensions.Html(@$"
            <html>
                <body>
                    <form action=""/login"" method=""POST"">
                        {formInputs}
                    </form>
                </body>
            </html>");
    }
}
