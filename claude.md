# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Verify.ImageSharp is a .NET library that extends the [Verify](https://github.com/VerifyTests/Verify) snapshot testing framework to support image verification via [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp). It registers stream/file converters so that Verify can compare image files (BMP, GIF, JPEG, PNG, TIFF) and `Image` objects, producing both metadata (`.txt`) and image (`.ext`) verified files.

## Build and Test Commands

```bash
# Build
dotnet build src --configuration Release

# Run all tests
dotnet test src --configuration Release

# Run a single test
dotnet test src/Tests --filter "FullyQualifiedName~Samples.VerifyImageFile"
```

The solution file is `src/Verify.ImageSharp.slnx`. Both projects target .NET 8.0 and require .NET SDK 10.0.200 (preview, see `src/global.json`).

## Architecture

The library consists of three files in `src/Verify.ImageSharp/`:

- **VerifyImageSharp.cs** — The single public API surface. `Initialize()` registers stream converters for each image format and a file converter for `Image` objects. Extension methods (`EncodeAsPng`, `EncodeAsJpeg`, etc.) on `VerifySettings`/`SettingsTask` allow tests to override the output encoding. Encoder/extension overrides are passed via the Verify context dictionary.
- **InfoBuilder.cs** — `GetInfo(Image)` extension that extracts metadata (dimensions, resolution) for the verified `.txt` output.
- **GlobalUsings.cs** — Imports for SixLabors.ImageSharp format namespaces.

Tests are in `src/Tests/` using NUnit. The `[ModuleInitializer]` pattern in `ModuleInit.cs` calls `VerifyImageSharp.Initialize()` before any test runs.

## Code Style

- `var` everywhere (enforced at error level)
- Expression-bodied members preferred
- File-scoped namespaces
- No accessibility modifiers on non-public members (`dotnet_style_require_accessibility_modifiers = never`)
- Private fields use camelCase
- Braces required for all control flow
- Warnings treated as errors; code style enforced in build
- LF line endings, 4-space indent for C#, 2-space for XML/JSON

## NuGet Package Management

Uses central package version management via `src/Directory.Packages.props`. Do not add `Version` attributes to `PackageReference` in `.csproj` files — versions are declared centrally.
