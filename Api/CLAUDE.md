# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Module Purpose

This is the **Api layer** of the Templafy Custom Content Connector - a reference implementation showing how to build a DAM (Digital Asset Management) connector for Templafy. The Api directory contains all HTTP endpoint handlers.

## Build & Run Commands

```bash
# Build
dotnet build

# Run locally (listens on http://localhost:7225)
dotnet run

# Build Docker image
docker build -t custom-content-connector .

# Run Docker container
docker run -p 7225:7225 custom-content-connector
```

## Architecture

**Framework:** .NET 8.0 ASP.NET Core with Minimal APIs (no controllers)

**Pattern:** All handlers are static classes with static methods - no dependency injection, pure functional style.

**Endpoint Mapping:** Defined in `Program.cs`, handlers in `Api/` directory:

| Endpoint | Handler | Purpose |
|----------|---------|---------|
| POST /oauth/token | `TokenApi.HandleTokenRequest` | OAuth token exchange |
| GET /oauth/authorize | `AuthorizationApi.RenderAuthorizationForm` | OAuth login form |
| GET /content | `ContentApi.HandleContentRequest` | List assets with pagination |
| GET /content/{assetId}/download-url | `ContentApi.HandleDownloadUrlRequest` | Get asset download URL |
| POST /login | `UserApi.HandleUserLogin` | Form submission handler |
| GET /download-asset/{assetId} | `ContentApi.DownloadAsset` | Serve asset files |

## Key Files

- `TokenApi.cs` - Handles all three OAuth flows: Client Credentials, Authorization Code, and PKCE
- `AuthorizationApi.cs` - Renders HTML login form for Authorization Code flows
- `ContentApi.cs` - Asset listing, pagination, and download; includes data models (`Asset`, `ContentResponse`)
- `UserApi.cs` - Processes login form submission

## Critical: Case Sensitivity

**All property names, query parameters, and MIME types are case-sensitive** per Templafy specification. JSON properties use `[JsonPropertyName]` attributes to ensure exact casing.

## OAuth Flows Supported

1. **Client Credentials** - Server-to-server with `client_id` + `client_secret`
2. **Authorization Code** - Traditional OAuth with user consent
3. **Authorization Code + PKCE** - Enhanced security, no client_secret required

## Test Credentials (Demo Only)

Defined in `Constants.cs`:
- Username: `user` / Password: `password`
- Client ID: `secure_client_id` / Secret: `secure_client_secret`
- Token: `secure_token`

## Dependencies

Zero external NuGet packages - uses only ASP.NET Core built-ins. `FakeStorage/Images/` provides sample assets for testing.

## Templafy MIME Types

From `Constants.TemplafyAcceptedMimeTypes`:
- `image/jpeg`, `image/png`, `image/svg+xml`
- `application/pdf`, `application/vnd.templafy.folder`
- Office formats: DOCX, PPTX
