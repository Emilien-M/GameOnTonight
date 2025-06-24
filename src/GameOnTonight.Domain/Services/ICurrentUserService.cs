namespace GameOnTonight.Domain.Services;

/// <summary>
/// Service for accessing the currently authenticated user.
/// </summary>
public interface ICurrentUserService
{
    /// <summary>
    /// Gets the ID of the current user.
    /// </summary>
    string? UserId { get; }
    
    /// <summary>
    /// Indicates whether the user is authenticated.
    /// </summary>
    bool IsAuthenticated { get; }
}