# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Templafy Custom Content Connector - a reference implementation demonstrating how to build a DAM (Digital Asset Management) connector for Templafy. This API allows external asset management systems to integrate with Templafy by implementing OAuth authentication and content serving endpoints.

## Build & Run Commands

```bash
# Build
dotnet build

# Run locally (http://localhost:7225)
dotnet run

# Docker
docker build -t custom-content-connector .
docker run -p 7225:7225 custom-content-connector
```

**No tests exist** - this is a reference implementation tested manually via Postman or Docker.

## Architecture

**Stack:** .NET 8.0 ASP.NET Core Minimal APIs (no controllers, no DI)

**Pattern:** Functional, static method handlers - all API classes are static with static methods.

**Zero dependencies** - uses only built-in ASP.NET Core libraries.

```
Program.cs          → Endpoint routing (6 routes)
Constants.cs        → Hard-coded demo credentials and MIME types
Api/                → HTTP handlers (TokenApi, ContentApi, AuthorizationApi, UserApi)
Utilities/          → Extensions (HtmlResponse)
FakeStorage/Images/ → Mock DAM storage (70+ sample images)
```

## Required Templafy Endpoints

| Endpoint | Purpose |
|----------|---------|
| `POST /oauth/token` | OAuth token exchange (3 grant types) |
| `GET /oauth/authorize` | Login form for Authorization Code flows |
| `GET /content` | List assets with pagination |
| `GET /content/{id}/download-url` | Get download URL for asset |

## Critical: Case Sensitivity

**All property names, query parameters, and MIME types are case-sensitive** per Templafy specification. JSON uses `[JsonPropertyName]` attributes for exact casing.

## OAuth Flows

1. **Client Credentials** - `grant_type=client_credentials` with `client_id` + `client_secret`
2. **Authorization Code** - `grant_type=authorization_code` with user login + `client_secret`
3. **Authorization Code + PKCE** - `grant_type=authorization_code` with code verifier (no secret)

## Demo Credentials

Defined in `Constants.cs` - for testing only:
- User: `user` / `password`
- Client: `secure_client_id` / `secure_client_secret`
- Token: `secure_token`

## Deployment

- **CI/CD:** Azure Pipelines (`azure-pipelines.yml`)
- **Target:** Azure Container Apps
- **Registry:** `customcontentconnectoracicontainerregistry`
- **Branch:** master (mainline versioning via GitVersion)

## Module Documentation

Subdirectories contain detailed CLAUDE.md files:
- `Api/CLAUDE.md` - Endpoint handlers, OAuth flows, data models
- `Utilities/CLAUDE.md` - HTML response extension
- `FakeStorage/CLAUDE.md` - Asset storage conventions
