using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.BoardGames.ViewModels;

/// <summary>
/// ViewModel representing a board game.
/// </summary>
public record BoardGameViewModel(
    int Id,
    string Name, 
    int MinPlayers, 
    int MaxPlayers, 
    int DurationMinutes, 
    string GameType,
    string? Description,
    string? ImageUrl,
    DateTime CreatedAt,
    DateTime? UpdatedAt
)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BoardGameViewModel"/> class.
    /// </summary>
    /// <param name="boardGame">The entity to map from.</param>
    public BoardGameViewModel(BoardGame boardGame)
        : this(
            boardGame.Id,
            boardGame.Name,
            boardGame.MinPlayers,
            boardGame.MaxPlayers,
            boardGame.DurationMinutes,
            boardGame.GameType,
            boardGame.Description,
            boardGame.ImageUrl,
            boardGame.CreatedAt,
            boardGame.UpdatedAt)
    {
    }
}
