using System.Text.Json.Serialization;

namespace CustomContentConnectorExample.Api;

public static class ContentApi
{
    /// <summary>
    /// <para>This request will be called by Templafy when the user uses the Connector.</para>
    /// <para>Templafy will call this Endpoint and pass the bearer token in the "Authorization" header.</para>
    /// </summary>
    /// <param name="request">The URL query parameters Templafy is sending for this request are deconstructed on lines 16-21.The <b>search</b> parameter is optional/ nullable
    /// and is only populated when the user is searching for a keyword.</param>
    /// <returns><para>All the values returned <b>are case sensitive</b></para></returns>
    public static IResult HandleContentRequest(HttpRequest request)
    {
        var query = request.Query;
        var skip = int.Parse(query["skip"]);
        var limit = int.Parse(query["limit"]);
        var contentType = query["contentType"];
        var search = query["search"];
        var parentId = query["parentId"];

        if (IsAuthorized(request) && contentType == "image")
        {
            //gets all files+directories for a given path and orders them in such a way that directories are first
            var entriesOnCurrentLevel = Directory.EnumerateFileSystemEntries($"./FakeStorage/Images/{parentId}")
                .Select(path => new FileInfo(path)).OrderBy(e => (e.Attributes & FileAttributes.Directory) == 0)
                .ToList();

            var filteredEntries = entriesOnCurrentLevel.Skip(skip).Take(limit).ToList();
            
            var assets = new List<Asset>();
            foreach (var fileInfo in filteredEntries)
            {
                var asset = new Asset
                {
                    Id = fileInfo.Name,
                    MimeType = GetMimeType(fileInfo),
                    Name = fileInfo.Name,
                    PreviewUrl = $"{request.Scheme}://{request.Host}/download-asset/{fileInfo.Name}",
                    Tags = "placeholder tag, other tag"
                };
                assets.Add(asset);
            }

            return Results.Ok(new ContentResponse
            {
                ContentCount = entriesOnCurrentLevel.Count,
                Offset = skip + assets.Count,
                Content = assets.ToArray()
            });
        }

        return Results.Unauthorized();
    }

    /// <summary>
    /// This endpoint will be called by Templafy when the users interacts with an Asset(ex: clicks the asset in order to insert it into a document)
    /// </summary>
    /// <param name="request"></param>
    /// <param name="assetId">The Id of the asset</param>
    /// <returns></returns>
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

    private static bool IsAuthorized(HttpRequest request)
    {
        var authorizationHeader = request.Headers.Authorization;

        // Since the value of this header is formatted as "Bearer <token>" we need to split it by space
        var authorizationHeaderParts = authorizationHeader.ToString().Split(' ');

        var authorizationHeaderSchema = authorizationHeaderParts[0];
        var token = authorizationHeaderParts[1];

        return authorizationHeaderSchema == "Bearer" && token == Constants.Token.Value;
    }

    private static string GetMimeType(FileInfo fileInfo)
    {
        return fileInfo switch
        {
            {Extension: ".svg"} => Constants.TemplafyAcceptedMimeTypes.Svg,
            {Extension: ".png"} => Constants.TemplafyAcceptedMimeTypes.Png,
            {Extension: ".jpg"} => Constants.TemplafyAcceptedMimeTypes.Jpg,
            {Attributes: FileAttributes.Directory} => Constants.TemplafyAcceptedMimeTypes.Folder,
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
}

public record DownloadUrlResponse
{
    [JsonPropertyName("downloadUrl")] public string DownloadUrl { get; set; }
}