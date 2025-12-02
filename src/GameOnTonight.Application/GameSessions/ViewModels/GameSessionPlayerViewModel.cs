using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.GameSessions.ViewModels;

public record GameSessionPlayerViewModel
{
    public int Id { get; init; }
    public string PlayerName { get; init; } = string.Empty;
    public int? Score { get; init; }
    public bool IsWinner { get; init; }
    public int? Position { get; init; }

    public GameSessionPlayerViewModel() { }

    public GameSessionPlayerViewModel(GameSessionPlayer entity)
    {
        Id = entity.Id;
        PlayerName = entity.PlayerName;
        Score = entity.Score;
        IsWinner = entity.IsWinner;
        Position = entity.Position;
    }
}
