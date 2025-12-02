using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.GameSessions.ViewModels;

public record GameSessionViewModel
{
    public int Id { get; init; }
    public int BoardGameId { get; init; }
    public string BoardGameName { get; init; } = string.Empty;
    public DateTime PlayedAt { get; init; }
    public int PlayerCount { get; init; }
    public string? Notes { get; init; }
    public int? Rating { get; init; }
    public string? PhotoUrl { get; init; }
    public List<GameSessionPlayerViewModel> Players { get; init; } = new();

    public GameSessionViewModel() { }

    public GameSessionViewModel(GameSession entity)
    {
        Id = entity.Id;
        BoardGameId = entity.BoardGameId;
        BoardGameName = entity.BoardGame?.Name ?? string.Empty;
        PlayedAt = entity.PlayedAt;
        PlayerCount = entity.PlayerCount;
        Notes = entity.Notes;
        Rating = entity.Rating;
        PhotoUrl = entity.PhotoUrl;
        Players = entity.Players?.Select(p => new GameSessionPlayerViewModel(p)).ToList() ?? new();
    }
}
