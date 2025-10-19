using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.BoardGames.ViewModels;

public record BoardGameViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int MinPlayers { get; init; }
    public int MaxPlayers { get; init; }
    public int DurationMinutes { get; init; }
    public string GameType { get; init; } = string.Empty;

    public BoardGameViewModel(BoardGame entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        MinPlayers = entity.MinPlayers;
        MaxPlayers = entity.MaxPlayers;
        DurationMinutes = entity.DurationMinutes;
        GameType = entity.GameType;
    }
}
