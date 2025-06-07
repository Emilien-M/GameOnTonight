using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.GameSessions.ViewModels;

/// <summary>
/// ViewModel représentant une partie jouée
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
    /// Constructeur qui effectue le mapping depuis une entité GameSession
    /// </summary>
    /// <param name="gameSession">L'entité à mapper</param>
    public GameSessionViewModel(GameSession gameSession)
        : this(
            gameSession.Id,
            gameSession.BoardGameId,
            gameSession.BoardGame?.Name ?? "Jeu inconnu",
            gameSession.PlayedAt,
            gameSession.PlayerCount,
            gameSession.Notes,
            gameSession.CreatedAt,
            gameSession.UpdatedAt)
    {
    }
}
