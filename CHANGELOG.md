# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

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

## [0.1.0] - 2024-XX-XX

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

[Unreleased]: https://github.com/Emilien-M/GameOnTonight/compare/v0.1.0...HEAD
[0.1.0]: https://github.com/Emilien-M/GameOnTonight/releases/tag/v0.1.0
