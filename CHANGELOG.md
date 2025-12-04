# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.3.1](https://github.com/Emilien-M/GameOnTonight/releases/tag/v0.3.1)

### Changed
- **BREAKING**: API is now served via `/api` path on the same origin as the app
  - Before: App at `https://domain.com/`, API at `https://domain.com:8080/`
  - After: App at `https://domain.com/`, API at `https://domain.com/api/`
- Nginx now acts as a reverse proxy for `/api` requests to the internal API container
- API container is no longer directly exposed (internal only via Docker network)
- Simplified environment variables:
  - Removed `CORS_ORIGIN` (no longer needed - same origin)
  - Removed `API_PORT` (API is internal only)
  - `API_BASE_URL` now defaults to `/api` (relative path)

### Added
- Support for relative API URLs in Blazor App (e.g., `/api` instead of absolute URLs)
- Reverse proxy configuration in nginx for `/api/*` routes

### Fixed
- CORS issues eliminated by serving app and API from the same origin

### Documentation
- Updated `DEPLOYMENT.md` with new architecture diagram
- Simplified reverse proxy and HTTPS configuration guides

## [0.3.0](https://github.com/Emilien-M/GameOnTonight/releases/tag/v0.3.0)

### Added
- Game Types feature: board games can now have multiple types (many-to-many relationship)
- New `GameTypesController` with full CRUD endpoints
- Chip-based suggestions UI for selecting game types in board game editor

### Changed
- **BREAKING**: Upgraded to .NET 10
  - Updated all 9 .csproj files to `net10.0`
  - Updated Dockerfiles to use .NET SDK 10.0 and ASP.NET 10.0-alpine
  - Updated GitHub Actions CI to use dotnet-version 10.0.x
- Filter logic now uses OR (match any selected type) instead of AND
- Replaced MudAutocomplete with MudTextField + chip suggestions in EditBoardGame
- Updated Library, FindForm, Result, and SuggestionModal to display multiple types

### Updated
- FluentValidation: 12.0.0 → 12.1.1
- System.IdentityModel.Tokens.Jwt: 8.14.0 → 8.15.0
- Scalar.AspNetCore: 2.9.0 → 2.11.0
- InfiniLore.Lucide: 0.548.0 → 0.555.0
- MudBlazor: 8.13.0 → 8.15.0
- bunit: 1.40.0 → 2.1.1
- Microsoft.NET.Test.Sdk: 17.12.0 → 18.0.1
- coverlet.collector: 6.0.2 → 6.0.4
- xunit: 2.9.2 → 2.9.3
- xunit.runner.visualstudio: 2.8.2 → 3.1.5
- Npgsql.EntityFrameworkCore.PostgreSQL: 9.x → 10.0.0

### Fixed
- BearerSecuritySchemeTransformer for OpenAPI breaking changes in .NET 10
- bunit 2.x breaking changes: `TestContext` → `BunitContext`, `IRenderedFragment` → `IRenderedComponent`
- Added `IAsyncLifetime` for async disposal compatibility with bunit 2.x + MudBlazor

### Documentation
- Translated Design.md to English for consistency
- Removed obsolete Spec.md file

## [0.2.0 First Release](https://github.com/Emilien-M/GameOnTonight/releases/tag/v0.2.0)

### Added
- Docker support for self-hosting (API and App Dockerfiles)
- Production docker-compose.yml with PostgreSQL, API, and App services
- Health check endpoints (`/health` and `/ready`)
- Auto-migration support via `AUTO_MIGRATE` environment variable
- Self-hosted Poppins font (removed Google Fonts CDN dependency)
- Comprehensive deployment documentation (`DEPLOYMENT.md`)
- GitHub Actions workflow for Docker image builds and publishing to ghcr.io
- Multi-architecture support (amd64, arm64)

### Changed
- Externalized all secrets from configuration files
- API now listens on port 8080 (cloud convention)

### Security
- Removed hardcoded database credentials
- Removed hardcoded JWT secret
- Added `.env` files to `.gitignore`

## [0.1.0]

### Added
- Initial release
- Board game collection management
- Game filtering by player count, duration, categories
- Random game suggestion for game nights
- User authentication with JWT
- RESTful API with ASP.NET Core
- Blazor WebAssembly PWA frontend
- PostgreSQL database support
- Clean Architecture structure

[Unreleased]: https://github.com/Emilien-M/GameOnTonight/compare/v0.3.1...HEAD
[0.3.1]: https://github.com/Emilien-M/GameOnTonight/compare/v0.3.0...v0.3.1
[0.3.0]: https://github.com/Emilien-M/GameOnTonight/compare/v0.2.0...v0.3.0
[0.2.0]: https://github.com/Emilien-M/GameOnTonight/releases/tag/v0.2.0
