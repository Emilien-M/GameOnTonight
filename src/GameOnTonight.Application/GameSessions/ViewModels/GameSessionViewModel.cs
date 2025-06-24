using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.GameSessions.ViewModels;

/// <summary>
/// ViewModel representing a played game.
/// </summary>
public record GameSessionViewModel(
    int Id,
    int BoardGameId,
    string BoardGameName,
    DateTime PlayedAt,
    int PlayerCount,
    string? Notes,
    DateTime CreatedAt,
    DateTime? UpdatedAt
)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameSessionViewModel"/> class.
    /// </summary>
    /// <param name="gameSession">The entity to map from.</param>
    public GameSessionViewModel(GameSession gameSession)
        : this(
            gameSession.Id,
            gameSession.BoardGameId,
            gameSession.BoardGame?.Name ?? "Unknown game",
            gameSession.PlayedAt,
            gameSession.PlayerCount,
            gameSession.Notes,
            gameSession.CreatedAt,
            gameSession.UpdatedAt)
    {
    }
}
