namespace GameOnTonight.Domain.Services;

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