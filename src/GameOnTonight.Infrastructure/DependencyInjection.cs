using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Interceptors;
using GameOnTonight.Infrastructure.Repositories;
using GameOnTonight.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GameOnTonight.Infrastructure;

/// <summary>
/// Extensions pour configurer les services liés aux repositories
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Ajoute les repositories au conteneur de services
    /// </summary>
    /// <param name="services">Collection de services</param>
    /// <returns>Collection de services mise à jour</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        // Enregistrement du service d'accès à l'utilisateur courant
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        // Enregistrement de l'intercepteur pour gérer les propriétés d'audit
        services.AddScoped<AuditableEntityInterceptor>();
        
        // Enregistrement des repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IBoardGameRepository, BoardGameRepository>();
        services.AddScoped<IGameSessionRepository, GameSessionRepository>();
        
        // Enregistrement de l'UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}