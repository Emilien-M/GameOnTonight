# GameOnTonight - AI Coding Agent Instructions

Board game collection management app with filtering and random game suggestions for game nights.

## Architecture Overview

**Clean Architecture** with 5 layers:
- **Domain** (`GameOnTonight.Domain`): Entities, interfaces (`IRepository`, `IUserOwnedEntity`), domain errors
- **Application** (`GameOnTonight.Application`): CQRS with Mediator, FluentValidation, ViewModels
- **Infrastructure** (`GameOnTonight.Infrastructure`): EF Core repositories, PostgreSQL, services
- **API** (`GameOnTonight.Api`): REST controllers with ASP.NET Core Identity auth
- **App** (`GameOnTonight.App`): Blazor WebAssembly PWA with MudBlazor

## Key Patterns

### CQRS Command/Query Structure
Commands and queries are **records** with handler in the **same file**:
```csharp
// Application/BoardGames/Commands/CreateBoardGameCommand.cs
public sealed record CreateBoardGameCommand(string Name, int MinPlayers...) : IRequest<BoardGameViewModel>;
public sealed class CreateBoardGameCommandValidator : AbstractValidator<CreateBoardGameCommand> { }
public sealed class CreateBoardGameCommandHandler : IRequestHandler<CreateBoardGameCommand, BoardGameViewModel> { }
```

### User-Owned Entities
All business entities inherit from `UserOwnedEntity` → data is **automatically filtered by `UserId`** in `Repository<T>`. Never query without this filter.

### Domain Validation
Entities self-validate using `AddDomainError()`, `ValidateString()`, `ValidateNumber()` from `BaseEntity`. The `UnitOfWork` throws `DomainException` if errors exist before `SaveChanges`.

### UnitOfWork Middleware
**Do NOT call `SaveChangesAsync()` in handlers**—the `UnitOfWorkMiddleware` commits automatically for non-GET successful requests.

## Development Commands

```bash
# Start PostgreSQL (from src/GameOnTonight.Infrastructure/Docker/)
docker-compose up -d

# Run API (from src/)
dotnet run --project GameOnTonight.Api

# Run Blazor App (from src/)
dotnet run --project GameOnTonight.App

# Regenerate REST client (API must be running on port 5235)
cd src/GameOnTonight.RestClient && ./generate-client.sh

# EF Migrations (from src/GameOnTonight.Infrastructure/Persistence/Migrations/)
dotnet ef migrations add <Name> -p ../GameOnTonight.Infrastructure -s ../GameOnTonight.Api -o Persistence/Migrations/
dotnet ef database update -p ../GameOnTonight.Infrastructure -s ../GameOnTonight.Api
```

## Naming Conventions

| Element | Pattern | Example |
|---------|---------|---------|
| Commands | `[Action][Entity]Command` | `CreateBoardGameCommand` |
| Queries | `[Action][Entity]Query` | `GetAllBoardGamesQuery` |
| ViewModels | `[Entity]ViewModel` | `BoardGameViewModel` |
| Repositories | `I[Entity]Repository` / `[Entity]Repository` | `IBoardGameRepository` |
| Async methods | `*Async` suffix | `LoadDataAsync` |

## Blazor App Conventions

- **API calls**: Use services in `App/Services/` that wrap `GameOnTonight.RestClient`—never call API directly from pages
- **Icons**: Lucide via `InfiniLore.Lucide` (`<LucideIcon Name="filter" />`)
- **UI Framework**: MudBlazor components
- **Auth tokens**: Stored via `Blazored.LocalStorage` (keys: `auth.accessToken`, `auth.refreshToken`)
- **Config**: `ApiBaseUrl` in `wwwroot/appsettings.json`

## File Structure for New Features

```
# Adding new entity "Wishlist":
Domain/Entities/Wishlist.cs                          # Entity inheriting UserOwnedEntity
Domain/Repositories/IWishlistRepository.cs           # Interface
Infrastructure/Repositories/WishlistRepository.cs    # Implementation
Application/Wishlists/ViewModels/WishlistViewModel.cs
Application/Wishlists/Commands/CreateWishlistCommand.cs  # Record + Validator + Handler
Application/Wishlists/Queries/GetAllWishlistsQuery.cs
Api/Controllers/WishlistsController.cs               # Inherits ControllerBase, [Authorize]
```

## Critical Rules

1. **ViewModels must have entity constructor**: `public BoardGameViewModel(BoardGame entity) { ... }`
2. **Controllers inherit `ControllerBase`** (not `Controller`)
3. **Files < 300 lines**; use `.razor.cs` code-behind when needed
4. **No CDN dependencies** in Blazor App—all assets in `wwwroot/lib/`
5. **Dark theme** default with Orange (#FCA311) accents
6. **Async methods**: Always pass `CancellationToken` as last parameter
