using Microsoft.Extensions.DependencyInjection;

namespace GameOnTonight.RestClient;

/// <summary>
/// Extensions for dependency injection services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the GameOnTonight API client to the services.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="baseUrl">The base URL of the API.</param>
    /// <param name="configureClient">Optional action to configure the HTTP client.</param>
    /// <param name="configureClientBuilder">Optional action to configure the HTTP client builder.</param>
    /// <returns>The service collection.</returns>
    public static IServiceCollection AddGameOnTonightApiClient(this IServiceCollection services, string baseUrl, Action<HttpClient>? configureClient = null, Action<IHttpClientBuilder>? configureClientBuilder = null)
    {
        services.AddHttpClient<Client>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        var httpClientBuilder = services.AddHttpClient("GameOnTonightApi", builder =>
        {
            builder.BaseAddress = new Uri(baseUrl);
            builder.DefaultRequestHeaders.Add("Accept", "application/json");
            configureClient?.Invoke(builder);
        });

        configureClientBuilder?.Invoke(httpClientBuilder);

        return services;
    }
}
