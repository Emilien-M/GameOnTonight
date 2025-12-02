namespace GameOnTonight.Application.Statistics.ViewModels;

/// <summary>
/// ViewModel containing all user statistics.
/// </summary>
public sealed record StatisticsViewModel
{
    /// <summary>
    /// Total number of games in the library.
    /// </summary>
    public int TotalGames { get; init; }

    /// <summary>
    /// Total number of game sessions played.
    /// </summary>
    public int TotalSessions { get; init; }

    /// <summary>
    /// Total number of unique players across all sessions.
    /// </summary>
    public int TotalUniquePlayers { get; init; }

    /// <summary>
    /// Top 5 most played games with play counts.
    /// </summary>
    public IReadOnlyList<TopGameViewModel> TopGames { get; init; } = [];

    /// <summary>
    /// Player statistics with win rates.
    /// </summary>
    public IReadOnlyList<PlayerStatViewModel> PlayerStats { get; init; } = [];

    /// <summary>
    /// Monthly play counts for the last 12 months.
    /// </summary>
    public IReadOnlyList<MonthlyPlayViewModel> MonthlyPlays { get; init; } = [];

    /// <summary>
    /// Average rating given to game sessions.
    /// </summary>
    public double? AverageRating { get; init; }
}

/// <summary>
/// Statistics for a top played game.
/// </summary>
public sealed record TopGameViewModel
{
    /// <summary>
    /// Name of the game.
    /// </summary>
    public string GameName { get; init; } = string.Empty;

    /// <summary>
    /// Number of times the game was played.
    /// </summary>
    public int PlayCount { get; init; }

    /// <summary>
    /// Average rating for this game's sessions.
    /// </summary>
    public double? AverageRating { get; init; }
}

/// <summary>
/// Statistics for a player.
/// </summary>
public sealed record PlayerStatViewModel
{
    /// <summary>
    /// Player's name.
    /// </summary>
    public string PlayerName { get; init; } = string.Empty;

    /// <summary>
    /// Total games participated.
    /// </summary>
    public int GamesPlayed { get; init; }

    /// <summary>
    /// Number of wins.
    /// </summary>
    public int Wins { get; init; }

    /// <summary>
    /// Win rate percentage (0-100).
    /// </summary>
    public double WinRate => GamesPlayed > 0 ? Math.Round((double)Wins / GamesPlayed * 100, 1) : 0;
}

/// <summary>
/// Monthly play statistics.
/// </summary>
public sealed record MonthlyPlayViewModel
{
    /// <summary>
    /// Month label (e.g., "Jan 2024").
    /// </summary>
    public string Month { get; init; } = string.Empty;

    /// <summary>
    /// Number of sessions played in this month.
    /// </summary>
    public int SessionCount { get; init; }
}
