using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.BoardGames.ViewModels;

public record BoardGameViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int MinPlayers { get; init; }
    public int MaxPlayers { get; init; }
    public int DurationMinutes { get; init; }
    public IReadOnlyList<string> GameTypes { get; init; } = [];

    // Sharing properties
    public int? GroupId { get; init; }
    public string? GroupName { get; init; }
    public bool IsShared { get; init; }
    public bool IsOwner { get; init; }

    public BoardGameViewModel(BoardGame entity, string? currentUserId = null)
    {
        Id = entity.Id;
        Name = entity.Name;
        MinPlayers = entity.MinPlayers;
        MaxPlayers = entity.MaxPlayers;
        DurationMinutes = entity.DurationMinutes;
        GameTypes = entity.GameTypes.Select(gt => gt.Name).ToList();

        // Sharing properties
        GroupId = entity.GroupId;
        GroupName = entity.Group?.Name;
        IsShared = entity.GroupId.HasValue;
        IsOwner = currentUserId != null && entity.UserId == currentUserId;
    }
}
