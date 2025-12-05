using GameOnTonight.RestClient.Models;

namespace GameOnTonight.App.Services;

/// <summary>
/// Service interface for profile operations.
/// </summary>
public interface IProfileService
{
    /// <summary>
    /// Gets the current user's profile with statistics.
    /// </summary>
    Task<ProfileViewModel?> GetProfileAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates the user's display name.
    /// </summary>
    Task<ProfileViewModel?> UpdateProfileAsync(string displayName, CancellationToken cancellationToken = default);
}
