using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Pipeline;
using GameOnTonight.AutoRest.Generated;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;

namespace GameOnTonight.AutoRest;

/// <summary>
/// Extensions pour les services d'injection de dépendances
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Ajoute le client API GameOnTonight généré par AutoRest (Azure.Core) aux services
    /// </summary>
    /// <param name="services">Collection de services</param>
    /// <param name="baseUrl">URL de base de l'API</param>
    /// <param name="configureClient">Action de configuration du client HTTP (optionnel)</param>
    /// <returns>Collection de services</returns>
    public static IServiceCollection AddGameOnTonightAzureCoreClient(
        this IServiceCollection services,
        string baseUrl,
        Action<GameOnTonightClient>? configureClient = null)
    {
        return services.AddSingleton(serviceProvider => 
        {
            // Configuration de base du client HTTP
            var options = new GameOnTonightClientOptions
            {
                Transport = new HttpClientTransport(new HttpClient()),
                Retry =
                {
                    Delay = TimeSpan.FromSeconds(2),
                    MaxRetries = 3,
                    Mode = RetryMode.Exponential
                }
            };
            
            // Création du client AutoRest avec Azure.Core
            var client = new GameOnTonightClient(new Uri(baseUrl), options);
            
            // Configuration additionnelle du client si fournie
            configureClient?.Invoke(client);
            
            return client;
        });
    }
    
    /// <summary>
    /// Ajoute le client API GameOnTonight généré par AutoRest (Azure.Core) aux services avec authentification
    /// </summary>
    /// <param name="services">Collection de services</param>
    /// <param name="baseUrl">URL de base de l'API</param>
    /// <param name="tokenProvider">Fonction fournissant le jeton d'accès pour l'authentification</param>
    /// <param name="configureClient">Action de configuration du client HTTP (optionnel)</param>
    /// <returns>Collection de services</returns>
    public static IServiceCollection AddGameOnTonightAzureCoreClientWithAuth(
        this IServiceCollection services,
        string baseUrl,
        Func<IServiceProvider, Task<string>> tokenProvider,
        Action<GameOnTonightClient>? configureClient = null)
    {
        services.AddScoped<BearerTokenCredential>(sp => 
            new BearerTokenCredential(async () => await tokenProvider(sp))
        );
        
        return services.AddSingleton<GameOnTonightClient>((serviceProvider) => 
        {
            // Récupération des credentials pour l'authentification
            var credentials = serviceProvider.GetRequiredService<BearerTokenCredential>();
            
            // Configuration de base du client HTTP
            var options = new GameOnTonightClientOptions
            {
                Transport = new HttpClientTransport(new HttpClient()),
                Retry =
                {
                    Delay = TimeSpan.FromSeconds(2),
                    MaxRetries = 3,
                    Mode = RetryMode.Exponential
                }
            };
            
            // Création du client AutoRest avec Azure.Core et credentials
            var client = new GameOnTonightClient(new Uri(baseUrl), credentials, options);
            
            // Configuration additionnelle du client si fournie
            configureClient?.Invoke(client);
            
            return client;
        });
    }
}

/// <summary>
/// Fournit des informations d'identification basées sur un jeton pour l'authentification avec Azure.Core
/// </summary>
public class BearerTokenCredential : TokenCredential
{
    private readonly Func<Task<string>> _tokenProvider;

    /// <summary>
    /// Initialise une nouvelle instance de la classe BearerTokenCredential
    /// </summary>
    /// <param name="tokenProvider">Fonction qui fournit le jeton d'accès</param>
    public BearerTokenCredential(Func<Task<string>> tokenProvider)
    {
        _tokenProvider = tokenProvider;
    }

    /// <inheritdoc />
    public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        return GetTokenAsync(requestContext, cancellationToken).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public override async ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
    {
        string token = await _tokenProvider();
        // Utilisation d'une date d'expiration arbitraire de 1 heure - dans un cas réel, vous devriez extraire cette information du jeton
        return new AccessToken(token, DateTimeOffset.UtcNow.AddHours(1));
    }
}
