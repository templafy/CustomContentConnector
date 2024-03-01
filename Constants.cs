namespace CustomContentConnectorExample;

public static class Constants
{
    public static class User
    {
        public const string Username = "user";
        public const string Password = "password";
    }

    public static class Token
    {
        public const string Value = "secure_token";
    }

    public static class Code
    {
        public const string Value = "secure_code";
    }

    public static class Secrets
    {
        public const string ClientId = "secure_client_id";
        public const string ClientSecret = "secure_client_secret";
    }

    /// <summary>
    /// These values are case sensitive
    /// </summary>
    public static class TemplafyAcceptedMimeTypes
    {
        public const string Folder = "application/vnd.templafy.folder";
        public const string Jpg = "image/jpeg";
        public const string Png = "image/png";
        public const string Svg = "image/svg+xml";
        public const string Pdf = "application/pdf";
        public const string Docx = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        public const string Pptx = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
    }
}