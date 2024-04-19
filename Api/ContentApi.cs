using System.Text.Json.Serialization;

namespace CustomContentConnectorExample.Api;

public static class ContentApi
{
    public static IResult HandleContentRequest(HttpRequest request)
    {
        var query = request.Query;
        var headers = request.Headers;
        var skip = int.Parse(query["skip"]);
        var limit = int.Parse(query["limit"]);
        var contentType = query["contentType"];
        var search = query["search"];
        var parentId = query["parentId"];

        var templafyUser = headers["x-TemplafyUser"];

        if (IsAuthorized(request) && contentType == "image")
        {
            var content = GetImages(request, search, parentId, skip, limit);
            return Results.Ok(content);
        }

        return Results.Unauthorized();
    }

    public static IResult HandleDownloadUrlRequest(HttpRequest request, string assetId)
    {
        return Results.Ok(new DownloadUrlResponse
        {
            DownloadUrl = $"{request.Scheme}://{request.Host}/download-asset/{assetId}"
        });
    }

    public static IResult DownloadAsset(HttpRequest request, string assetId)
    {
        var foundFiles = Directory.GetFiles("./FakeStorage/Images", assetId, SearchOption.AllDirectories);
        Stream fileStream = File.OpenRead(foundFiles.First());
        return Results.File(fileStream);
    }


    private static ContentResponse GetImages(HttpRequest request, string search, string parentId, int skip, int limit)
    {
        //gets all files+directories for a given path and orders them in such a way that directories are first
        var entriesOnCurrentLevel = Directory.EnumerateFileSystemEntries($"./FakeStorage/Images/{parentId}")
            .Select(path => new FileInfo(path)).OrderBy(e => (e.Attributes & FileAttributes.Directory) == 0)
            .ToList();

        var filteredEntries = entriesOnCurrentLevel.Skip(skip).Take(limit).ToList();

        var assets = new List<Asset>();
        foreach (var fileInfo in filteredEntries)
        {
            var mimeType = GetMimeType(fileInfo);
            var supportsAltText = Constants.TemplafyAcceptedMimeTypes.AssetsAcceptingMimeType.Contains(mimeType);
            assets.Add(new Asset
            {
                Id = fileInfo.Name,
                MimeType = mimeType,
                Name = fileInfo.Name,
                PreviewUrl = $"{request.Scheme}://{request.Host}/download-asset/{fileInfo.Name}",
                Tags = "placeholder tag, other tag",
                AltText = supportsAltText ? "Description of image" : null,
            });
        }

        return new ContentResponse
        {
            ContentCount = entriesOnCurrentLevel.Count,
            Offset = skip + assets.Count,
            Content = assets.ToArray()
        };
    }

    private static bool IsAuthorized(HttpRequest request)
    {
        var authorizationHeader = request.Headers.Authorization;

        // Since the value of this header is formatted as "Bearer {token}" we need to split it by space
        var authorizationHeaderParts = authorizationHeader.ToString().Split(' ');

        var authorizationHeaderSchema = authorizationHeaderParts[0];
        var token = authorizationHeaderParts[1];

        return authorizationHeaderSchema == "Bearer" && token == Constants.Token.Value;
    }

    private static string GetMimeType(FileInfo fileInfo)
    {
        return fileInfo switch
        {
            { Extension: ".svg" } => Constants.TemplafyAcceptedMimeTypes.Svg,
            { Extension: ".png" } => Constants.TemplafyAcceptedMimeTypes.Png,
            { Extension: ".jpg" } => Constants.TemplafyAcceptedMimeTypes.Jpg,
            { Attributes: FileAttributes.Directory } => Constants.TemplafyAcceptedMimeTypes.Folder,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public record ContentResponse
{
    [JsonPropertyName("contentCount")] public int ContentCount { get; set; }

    [JsonPropertyName("offset")] public int Offset { get; set; }

    [JsonPropertyName("content")] public Asset[] Content { get; set; }
}

public record Asset
{
    [JsonPropertyName("id")] public string Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("mimeType")] public string MimeType { get; set; }

    [JsonPropertyName("previewUrl")] public string? PreviewUrl { get; set; }

    [JsonPropertyName("tags")] public string? Tags { get; set; }

    [JsonPropertyName("altText")] public string? AltText { get; set; }
}

public record DownloadUrlResponse
{
    [JsonPropertyName("downloadUrl")] public string DownloadUrl { get; set; }
}
