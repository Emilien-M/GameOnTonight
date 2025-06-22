# GameOnTonight AutoRest Client

Ce projet contient un client HTTP généré automatiquement via AutoRest pour communiquer avec l'API GameOnTonight. Il utilise la bibliothèque moderne Azure.Core au lieu de Microsoft.Rest.ClientRuntime qui est obsolète.

## À propos d'AutoRest

AutoRest est un outil développé par Microsoft qui génère des bibliothèques clientes pour accéder aux API REST à partir de spécifications OpenAPI (Swagger). Il prend en charge plusieurs langages dont C#, TypeScript, Python, Java, etc.

## Génération du client API

Le client API est généré automatiquement à partir du document OpenAPI (Swagger) exposé par l'API GameOnTonight.

### Prérequis

- .NET 9 SDK
- PowerShell 7.0 ou supérieur
- Node.js et npm (pour installer AutoRest)
- L'API GameOnTonight doit être en cours d'exécution pour la génération

### Installation d'AutoRest

```bash
# Installation d'AutoRest en global via npm
npm install -g autorest
```

### Génération du client

Pour générer le client API moderne basé sur Azure.Core, exécutez le script PowerShell :

```bash
pwsh ./generate-client-azure.ps1
```

Le script vérifiera la disponibilité de l'API, téléchargera le document OpenAPI, installera AutoRest si nécessaire, et générera les fichiers du client dans le dossier `Generated/`.

### Options de la commande

Le script accepte plusieurs paramètres pour personnaliser la génération :

```bash
pwsh ./generate-client-azure.ps1 -ApiUrl "http://localhost:5235" -OpenApiEndpoint "/openapi/v1.json" -OutputFolder "./Generated" -Namespace "GameOnTonight.AutoRest.Generated" -ClientName "GameOnTonightClient"
```

## Utilisation du client dans une application .NET

### Configuration dans Program.cs

Pour configurer le client API dans votre application .NET, ajoutez le code suivant dans votre fichier `Program.cs` :

```csharp
using GameOnTonight.AutoRest;
using GameOnTonight.AutoRest.Generated;

// ...

// Configuration du client API sans authentification
builder.Services.AddGameOnTonightAzureCoreClient(
    baseUrl: "https://api.gametonight.example.com",
    configureClient: client => 
    {
        // Configuration additionnelle du client si nécessaire
    });

// OU avec authentification
builder.Services.AddGameOnTonightAzureCoreClientWithAuth(
    baseUrl: "https://api.gametonight.example.com",
    tokenProvider: async serviceProvider => 
    {
        // Logique pour récupérer le jeton d'accès
        var authService = serviceProvider.GetRequiredService<IAuthService>();
        var token = await authService.GetAccessTokenAsync();
        return token;
    },
    configureClient: client => 
    {
        // Configuration additionnelle du client si nécessaire
    });
```

### Utilisation dans un service ou un contrôleur

Pour utiliser le client API dans un service ou un contrôleur :

```csharp
public class BoardGameService
{
    private readonly GameOnTonightClient _client;

    public BoardGameService(GameOnTonightClient client)
    {
        _client = client;
    }

    public async Task<IEnumerable<BoardGameModel>> GetBoardGamesAsync()
    {
        try
        {
            return await _client.GetBoardGamesAsync();
        }
        catch (RequestFailedException ex)
        {
            // Gestion des erreurs API avec Azure.Core
            Console.Error.WriteLine($"Erreur API: {ex.Status} - {ex.Message}");
            throw;
        }
    }
}
```

## Différences avec NSwag

AutoRest et NSwag sont deux outils similaires pour générer des clients API, mais avec quelques différences clés :

### AutoRest :
- Développé par Microsoft et utilisé intensivement pour Azure SDK
- Support solide du protocole OpenAPI
- Génère désormais du code avec la bibliothèque Azure.Core (moderne et maintenue)
- Architecture optimisée pour les performances et la fiabilité
- Politiques de nouvelle tentative intégrées

### NSwag :
- Plus léger et flexible
- Génère des clients qui utilisent directement HttpClient
- Intégration simple avec ASP.NET Core
- Possibilité de générer aussi des clients pour le frontend (Angular, Axios, etc.)

## À propos d'Azure.Core

Azure.Core est une bibliothèque moderne qui remplace Microsoft.Rest.ClientRuntime (qui est obsolète). Voici ses principaux avantages :

1. **Activement maintenue** : Azure.Core est la base de tous les nouveaux SDK Azure
2. **Performances** : Optimisée pour les performances avec des allocations mémoire réduites
3. **Gestion des erreurs** : Meilleur modèle d'exceptions avec la classe `RequestFailedException`
4. **Politiques de retry** : Configurations flexibles pour les nouvelles tentatives automatiques
5. **Support moderne** : Utilisation des fonctionnalités récentes de .NET comme `System.Text.Json` et `ValueTask`

## Gestion des erreurs

Le client AutoRest basé sur Azure.Core génère des exceptions de type `RequestFailedException` en cas d'erreur lors de la communication avec l'API.

```csharp
try
{
    var result = await _client.GetBoardGameAsync(id);
    return result;
}
catch (RequestFailedException ex) when (ex.Status == 404)
{
    // Ressource non trouvée
    return null;
}
catch (RequestFailedException ex)
{
    // Autres erreurs API
    _logger.LogError($"Erreur API {ex.Status}: {ex.Message}");
    throw;
}
catch (Exception ex)
{
    // Erreurs non liées à l'API
    _logger.LogError(ex, "Une erreur est survenue lors de l'appel à l'API");
    throw;
}
```

## Politiques de nouvelles tentatives (Retry)

Le client généré avec Azure.Core peut être configuré avec des politiques de nouvelles tentatives sophistiquées :

```csharp
// Dans Program.cs
builder.Services.AddGameOnTonightAzureCoreClient(
    baseUrl: "https://api.gametonight.example.com",
    configureClient: client => 
    {
        // Configuration personnalisée des options de retry
        if (client.Options is GameOnTonightClientOptions options)
        {
            options.Retry.MaxRetries = 5;
            options.Retry.Delay = TimeSpan.FromSeconds(1);
            options.Retry.Mode = Azure.Core.RetryMode.Exponential;
        }
    });
```

## Mise à jour du client

À chaque modification de l'API (nouveaux endpoints, changements de modèles, etc.), il suffit de régénérer le client API en exécutant à nouveau le script `generate-client-azure.ps1` pour maintenir la synchronisation entre le client et l'API.
