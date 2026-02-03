# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Module Purpose

The Utilities layer provides shared helper extensions for the Custom Content Connector API. Currently contains HTTP response utilities for ASP.NET Core minimal APIs.

## Key Components

### HtmlResponse.cs
Extension method `Results.Extensions.Html(string html)` for returning HTML content from minimal API endpoints. Used by the OAuth `/authorize` endpoint to render the login form. Implements `IResult` with proper content-type headers and UTF-8 encoding.

## Usage Pattern

```csharp
return Results.Extensions.Html("<html>...</html>");
```

## Dependencies

- ASP.NET Core minimal APIs (`IResult`, `IResultExtensions`, `HttpContext`)
- .NET 8.0
