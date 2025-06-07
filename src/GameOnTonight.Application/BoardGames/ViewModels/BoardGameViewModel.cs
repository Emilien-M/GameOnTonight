using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.BoardGames.ViewModels;

/// <summary>
/// ViewModel représentant un jeu de société
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
    /// Constructeur qui effectue le mapping depuis une entité BoardGame
    /// </summary>
    /// <param name="boardGame">L'entité à mapper</param>
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
