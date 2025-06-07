# GameOnTonight REST Client

Ce projet contient un client HTTP généré automatiquement pour communiquer avec l'API GameOnTonight.

## Génération du client API

Le client API est généré automatiquement à partir du document OpenAPI (Swagger) exposé par l'API GameOnTonight.

### Prérequis

- .NET 9 SDK
- PowerShell 7.0 ou supérieur
- L'API GameOnTonight doit être en cours d'exécution pour la génération

### Installation de l'outil NSwag

```bash
# Installation de l'outil NSwag en global
dotnet tool install -g NSwag.ConsoleCore
```

Ou en local dans le projet :

```bash
dotnet new tool-manifest # Si vous n'avez pas encore de fichier .config/dotnet-tools.json
dotnet tool install NSwag.ConsoleCore
```

### Génération du client

Pour générer le client API, exécutez le script PowerShell :

```bash
pwsh ./generate-client.ps1
```

Le script générera un fichier `ApiClient.cs` dans le dossier `Generated/`.

## Utilisation du client dans une application Blazor

### Configuration dans Program.cs

Pour configurer le client API dans votre application Blazor, ajoutez le code suivant dans votre fichier `Program.cs` :

```csharp
using GameOnTonight.RestClient;

// ...

// Configuration du client API
builder.Services.AddGameOnTonightApiClient(
    baseUrl: "https://api.gametonight.example.com",
    configureClient: client => 
    {
        // Configuration additionnelle du HttpClient si nécessaire
        client.DefaultRequestHeaders.Add("Accept", "application/json");
        
        // Pour ajouter un jeton d'authentification
        // client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "votre-token");
    });
```

### Utilisation dans un composant Blazor

Pour utiliser le client API dans un composant Blazor :

```csharp
@page "/exemple"
@using GameOnTonight.RestClient.Generated
@inject IGameOnTonightApiClient ApiClient

<h1>Exemple d'utilisation du client API</h1>

@if (isLoading)
{
    <p>Chargement...</p>
}
else
{
    <ul>
        @foreach (var item in items)
        {
            <li>@item.Name</li>
        }
    </ul>
}

@code {
    private bool isLoading = true;
    private ICollection<ItemDto> items = new List<ItemDto>();

    protected override async Task OnInitializedAsync()
    {
        try
        {
            isLoading = true;
            items = await ApiClient.GetItemsAsync();
        }
        catch (ApiException ex)
        {
            // Gestion des erreurs API
            Console.Error.WriteLine($"Erreur API: {ex.StatusCode} - {ex.Message}");
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }
}
```

### Gestion des erreurs

Le client API généré lèvera des exceptions de type `ApiException` en cas d'erreur de communication avec l'API. 
Cette exception contient des informations utiles comme le code de statut HTTP et le contenu de la réponse.

```csharp
try
{
    var result = await ApiClient.MethodeAsync();
}
catch (ApiException ex)
{
    // Accès aux détails de l'erreur
    int statusCode = ex.StatusCode;
    string responseContent = ex.Response;
    
    // Gestion spécifique selon le code de statut
    if (ex.StatusCode == 401)
    {
        // Non autorisé - rediriger vers la page de connexion
    }
    else if (ex.StatusCode == 404)
    {
        // Ressource non trouvée
    }
}
```

## Avantages de cette approche

1. **Simplicité** : Le client API est généré directement à partir de l'API sans couche d'abstraction supplémentaire.
2. **Maintenabilité** : Le code généré contient toute la logique nécessaire pour communiquer avec l'API.
3. **Robustesse** : Le client est fortement typé et inclut toutes les méthodes correspondant aux endpoints de l'API.

## Mise à jour du client

À chaque modification de l'API (nouveaux endpoints, changements de modèles, etc.), il suffit de régénérer le client API pour maintenir la synchronisation entre le client et l'API.
