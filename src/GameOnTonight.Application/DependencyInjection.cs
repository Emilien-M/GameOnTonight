using Microsoft.Extensions.DependencyInjection;

namespace GameOnTonight.Application;

/// <summary>
/// Extensions pour configurer les services liés à la couche Application
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Ajoute les services de la couche Application au conteneur de services
    /// </summary>
    /// <param name="services">Collection de services</param>
    /// <returns>Collection de services mise à jour</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Enregistrement des handlers Mediator se fait automatiquement via le service AddMediator
        // qui scanne les assemblies pour trouver les handlers
        
        return services;
    }
}
