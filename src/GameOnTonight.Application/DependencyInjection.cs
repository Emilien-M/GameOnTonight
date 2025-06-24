using Microsoft.Extensions.DependencyInjection;

namespace GameOnTonight.Application;

/// <summary>
/// Extensions for configuring services related to the Application layer.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds the Application layer services to the service container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        return services;
    }
}
