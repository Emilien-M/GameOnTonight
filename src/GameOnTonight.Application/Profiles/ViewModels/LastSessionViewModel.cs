namespace GameOnTonight.Application.Profiles.ViewModels;

/// <summary>
/// ViewModel representing the last played game session.
/// </summary>
public sealed record LastSessionViewModel
{
    /// <summary>
    /// Name of the game that was played.
    /// </summary>
    public string GameName { get; init; } = string.Empty;

    /// <summary>
    /// Date and time when the game was played.
    /// </summary>
    public DateTime PlayedAt { get; init; }

    /// <summary>
    /// Number of players in the session.
    /// </summary>
    public int PlayerCount { get; init; }

    /// <summary>
    /// Rating given to the session (1-5), if any.
    /// </summary>
    public int? Rating { get; init; }

    /// <summary>
    /// Human-readable time ago string (e.g., "Il y a 3 jours").
    /// </summary>
    public string TimeAgo { get; init; } = string.Empty;
}
