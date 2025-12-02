# Contributing to GameOnTonight

Merci de votre intérêt pour contribuer à GameOnTonight ! Ce document vous guidera pour configurer votre environnement de développement et contribuer au projet.

## 📋 Table des matières

- [Prérequis](#prérequis)
- [Configuration de l'environnement](#configuration-de-lenvironnement)
- [Architecture](#architecture)
- [Conventions de code](#conventions-de-code)
- [Structure des fichiers](#structure-des-fichiers)
- [Tests](#tests)
- [Processus de Pull Request](#processus-de-pull-request)

## 🔧 Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker](https://www.docker.com/) (pour PostgreSQL)
- [Kiota](https://learn.microsoft.com/en-us/openapi/kiota/) (pour générer le client REST)
- Un IDE compatible (VS Code, Rider, Visual Studio)

## 🚀 Configuration de l'environnement

### 1. Cloner le repository

```bash
git clone https://github.com/Emilien-M/GameOnTonight.git
cd GameOnTonight
```

### 2. Démarrer la base de données PostgreSQL

```bash
cd src/GameOnTonight.Infrastructure/Docker
docker-compose up -d
```

### 3. Appliquer les migrations

```bash
cd src
dotnet ef database update -p GameOnTonight.Infrastructure -s GameOnTonight.Api
```

### 4. Lancer l'API

```bash
cd src
dotnet run --project GameOnTonight.Api
```

L'API sera accessible sur `http://localhost:5235`.

### 5. Lancer l'application Blazor (dans un autre terminal)

```bash
cd src
dotnet run --project GameOnTonight.App
```

L'application sera accessible sur `http://localhost:5000` (ou le port indiqué).

### 6. Régénérer le client REST (si l'API a changé)

L'API doit être en cours d'exécution :

```bash
cd src/GameOnTonight.RestClient
./generate-client.sh
```

## 🏗️ Architecture

GameOnTonight suit une **Clean Architecture** avec 5 couches :

```
┌─────────────────────────────────────────────────────────┐
│                    GameOnTonight.App                     │
│              (Blazor WebAssembly PWA)                    │
└─────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────┐
│                   GameOnTonight.Api                      │
│           (REST Controllers, ASP.NET Core)               │
└─────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────┐
│               GameOnTonight.Application                  │
│          (CQRS, Mediator, FluentValidation)              │
└─────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────┐
│               GameOnTonight.Infrastructure               │
│            (EF Core, Repositories, Services)             │
└─────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────┐
│                  GameOnTonight.Domain                    │
│         (Entities, Interfaces, Domain Errors)            │
└─────────────────────────────────────────────────────────┘
```

### Pattern CQRS

Les Commands et Queries sont des **records** avec leur Handler dans le **même fichier** :

```csharp
// Application/BoardGames/Commands/CreateBoardGameCommand.cs
public sealed record CreateBoardGameCommand(string Name, int MinPlayers...) : IRequest<BoardGameViewModel>;

public sealed class CreateBoardGameCommandValidator : AbstractValidator<CreateBoardGameCommand> { }

public sealed class CreateBoardGameCommandHandler : IRequestHandler<CreateBoardGameCommand, BoardGameViewModel> { }
```

### User-Owned Entities

Toutes les entités métier héritent de `UserOwnedEntity` → les données sont **automatiquement filtrées par `UserId`** dans le `Repository<T>`.

### UnitOfWork Middleware

**Ne PAS appeler `SaveChangesAsync()` dans les handlers** — le `UnitOfWorkMiddleware` commit automatiquement pour les requêtes non-GET réussies.

## 📝 Conventions de code

### Nommage

| Élément | Pattern | Exemple |
|---------|---------|---------|
| Commands | `[Action][Entity]Command` | `CreateBoardGameCommand` |
| Queries | `[Action][Entity]Query` | `GetAllBoardGamesQuery` |
| ViewModels | `[Entity]ViewModel` | `BoardGameViewModel` |
| Repositories | `I[Entity]Repository` / `[Entity]Repository` | `IBoardGameRepository` |
| Méthodes async | Suffixe `*Async` | `LoadDataAsync` |

### ViewModels

Les ViewModels **doivent** avoir un constructeur prenant l'entité :

```csharp
public record BoardGameViewModel
{
    public int Id { get; init; }
    // ... autres propriétés

    public BoardGameViewModel(BoardGame entity)
    {
        Id = entity.Id;
        // ... mapping
    }
}
```

### Controllers

- Héritent de `ControllerBase` (pas `Controller`)
- Toujours décorés avec `[Authorize]` sauf endpoints publics

### Méthodes asynchrones

- Toujours passer `CancellationToken` comme dernier paramètre
- Suffixer avec `Async`

### Fichiers

- Maximum **300 lignes** par fichier
- Utiliser `.razor.cs` code-behind quand le composant devient trop long

### Blazor App

- **Appels API** : Utiliser les services dans `App/Services/` qui encapsulent le `RestClient`
- **Icônes** : Lucide via `InfiniLore.Lucide` (`<LucideIcon Name="filter" />`)
- **UI** : Composants MudBlazor
- **Thème** : Dark theme par défaut avec accents Orange (#FCA311)

## 📁 Structure des fichiers pour nouvelles fonctionnalités

Exemple pour ajouter une entité "Wishlist" :

```
Domain/Entities/Wishlist.cs                          # Entity héritant de UserOwnedEntity
Domain/Repositories/IWishlistRepository.cs           # Interface
Infrastructure/Repositories/WishlistRepository.cs    # Implémentation
Application/Wishlists/ViewModels/WishlistViewModel.cs
Application/Wishlists/Commands/CreateWishlistCommand.cs  # Record + Validator + Handler
Application/Wishlists/Queries/GetAllWishlistsQuery.cs
Api/Controllers/WishlistsController.cs               # Hérite de ControllerBase, [Authorize]
App/Services/IWishlistService.cs
App/Services/WishlistService.cs
App/Pages/Wishlist/Wishlist.razor
```

## 🧪 Tests

### Lancer les tests

```bash
cd src
dotnet test
```

### Structure des tests

- `tests/GameOnTonight.Domain.Tests/` - Tests unitaires du domaine
- `tests/GameOnTonight.Application.Tests/` - Tests des handlers CQRS
- `tests/GameOnTonight.App.Tests/` - Tests des composants Blazor

### Écrire des tests

- Utiliser le pattern **Arrange-Act-Assert**
- Nommer les tests : `MethodName_StateUnderTest_ExpectedBehavior`

## 🔀 Processus de Pull Request

### 1. Créer une branche

```bash
git checkout -b feature/ma-fonctionnalite
# ou
git checkout -b fix/mon-bugfix
```

### 2. Conventions de commits

Utiliser des messages descriptifs :

```
feat: ajouter le filtrage par type de jeu
fix: corriger l'affichage des joueurs min/max
docs: mettre à jour le README
refactor: simplifier le BoardGamesService
```

### 3. Checklist avant PR

- [ ] Le code compile sans warnings
- [ ] Les tests passent
- [ ] Le nouveau code a des tests
- [ ] Le REST Client est régénéré si l'API a été modifiée
- [ ] Les ViewModels ont un constructeur avec l'entité
- [ ] Les méthodes async ont `CancellationToken`
- [ ] Pas de `SaveChangesAsync()` dans les handlers
- [ ] Les Controllers héritent de `ControllerBase`

### 4. Soumettre la PR

- Titre clair et descriptif
- Description des changements
- Référencer les issues concernées (`Fixes #123`)

## ❓ Questions ?

Si vous avez des questions, n'hésitez pas à ouvrir une issue sur GitHub !

---

Merci de contribuer à GameOnTonight ! 🎲
