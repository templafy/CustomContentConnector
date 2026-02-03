# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Module Purpose

FakeStorage is the **data/asset directory** for the Custom Content Connector example. It contains sample image assets that simulate a DAM (Digital Asset Management) system backend. This directory has no code - it serves as a mock file-system storage layer accessed by the `ContentApi` in the parent directory.

## Directory Structure

```
FakeStorage/
  Images/
    ├── *.png files (70+ sample images)
    └── FolderExample/    # Subdirectory to demonstrate folder navigation
```

## How Assets Are Accessed

The `ContentApi.cs` (in `../Api/`) uses file system operations to serve content:

- **Listing**: `Directory.EnumerateFileSystemEntries("./FakeStorage/Images/{parentId}")`
- **Download**: `Directory.GetFiles("./FakeStorage/Images", assetId, SearchOption.AllDirectories)`
- **MIME detection**: Based on file extension (`.png`, `.jpg`, `.svg`) or `FileAttributes.Directory` for folders

## Asset Conventions

- Files are served directly as image assets with MIME types `image/png`, `image/jpeg`, `image/svg+xml`
- Directories are treated as folders with MIME type `application/vnd.templafy.folder`
- Filenames are used as asset IDs
- The `FolderExample/` subdirectory demonstrates nested folder navigation via `parentId` parameter

## Adding/Modifying Assets

When adding test assets:
- Place image files directly in `FakeStorage/Images/`
- Create subdirectories to test folder navigation
- Supported formats: PNG, JPG, SVG
- Asset files are copied to build output via the project file's `<CopyToOutputDirectory>Always</CopyToOutputDirectory>` directive
