using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace GameOnTonight.Infrastructure.Services;

/// <summary>
/// Service pour accéder à l'utilisateur actuellement authentifié
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Obtient l'identifiant de l'utilisateur courant
    /// </summary>
    string? UserId { get; }
    
    /// <summary>
    /// Indique si l'utilisateur est authentifié
    /// </summary>
    bool IsAuthenticated { get; }
}

/// <summary>
/// Implémentation du service d'utilisateur courant utilisant HttpContext
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
