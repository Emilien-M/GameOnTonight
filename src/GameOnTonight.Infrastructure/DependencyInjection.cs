using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Interceptors;
using GameOnTonight.Infrastructure.Repositories;
using GameOnTonight.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GameOnTonight.Infrastructure;

/// <summary>
/// Extensions for configuring services related to repositories.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds repositories to the service container.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>The updated service collection.</returns>
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        
        services.AddScoped<IEntityValidationService, EntityValidationService>();
        
        services.AddScoped<AuditableEntityInterceptor>();
        
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IBoardGameRepository, BoardGameRepository>();
        services.AddScoped<IGameSessionRepository, GameSessionRepository>();
        services.AddScoped<IGameTypeRepository, GameTypeRepository>();
        services.AddScoped<IProfileRepository, ProfileRepository>();
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}