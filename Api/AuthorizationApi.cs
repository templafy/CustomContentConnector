using CustomContentConnectorExample.Utilities;

namespace CustomContentConnectorExample.Api;

public static class AuthorizationApi
{
    public static IResult RenderAuthorizationForm(HttpRequest request)
    {
        var query = request.Query;

        var responseType = query["response_type"];
        var clientId = query["client_id"];
        var state = query["state"];
        var scope = query["scope"];
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
