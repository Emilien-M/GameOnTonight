using GameOnTonight.RestClient.Generated;
using Microsoft.Extensions.DependencyInjection;

namespace GameOnTonight.RestClient;

/// <summary>
/// Extensions pour les services d'injection de dépendances
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Ajoute le client API GameOnTonight aux services
    /// </summary>
    /// <param name="services">Collection de services</param>
    /// <param name="baseUrl">URL de base de l'API</param>
    /// <param name="configureClient">Action de configuration du client HTTP (optionnel)</param>
    /// <param name="handler">Fonction pour obtenir un DelegatingHandler personnalisé (optionnel)</param>
    /// <returns>Collection de services</returns>
    public static IServiceCollection AddGameOnTonightApiClient(
        this IServiceCollection services, 
        string baseUrl,
        Action<HttpClient>? configureClient = null,
        Func<IServiceProvider, DelegatingHandler>? handler = null)
    {
        var builder = services.AddHttpClient<IGameOnTonightApiClient, GameOnTonightApiClient>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
            configureClient?.Invoke(client);
        });

        // Ajout du gestionnaire de message personnalisé si fourni
        if (handler != null)
        {
            builder.ConfigureAdditionalHttpMessageHandlers((handlers, serviceProvider) => 
            {
                handlers.Add(handler(serviceProvider));
            });
        }

        return services;
    }
}
