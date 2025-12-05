namespace GameOnTonight.Application.Profiles.ViewModels;

/// <summary>
/// ViewModel containing user profile information and statistics.
/// </summary>
public sealed record ProfileViewModel
{
    /// <summary>
    /// User's display name.
    /// </summary>
    public string DisplayName { get; init; } = string.Empty;

    /// <summary>
    /// Date when the user created their account.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Human-readable member since string (e.g., "Membre depuis 2 ans").
    /// </summary>
    public string MemberSince { get; init; } = string.Empty;

    /// <summary>
    /// Avatar initials derived from display name (e.g., "JD" for "John Doe").
    /// </summary>
    public string AvatarInitials { get; init; } = string.Empty;

    /// <summary>
    /// Total number of games in the user's library.
    /// </summary>
    public int TotalGames { get; init; }

    /// <summary>
    /// Total number of game sessions played.
    /// </summary>
    public int TotalSessions { get; init; }

    /// <summary>
    /// User's win rate percentage (0-100), null if not available.
    /// </summary>
    public double? WinRate { get; init; }

    /// <summary>
    /// Average rating given to game sessions.
    /// </summary>
    public double? AverageRating { get; init; }

    /// <summary>
    /// Information about the last played game session, if any.
    /// </summary>
    public LastSessionViewModel? LastSession { get; init; }
}
