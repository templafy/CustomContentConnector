﻿using CustomContentConnectorExample.Api;

var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.MapPost("/oauth/token", TokenApi.HandleTokenRequest);
app.MapGet("/oauth/authorize", AuthorizationApi.RenderAuthorizationForm);
app.MapGet("/content", ContentApi.HandleContentRequest);
app.MapGet("/content/{assetId}/download-url", ContentApi.HandleDownloadUrlRequest);

// The next two endpoints are not part of the Templafy API specifications, however they are needed in order make this entire example work
app.MapPost("/login", UserApi.HandleUserLogin);      // This is used to complete the flow of "/oauth/authorize" in a proof of concept manner.
app.MapGet("/download-asset/{assetId}", ContentApi.DownloadAsset);

app.Run();
