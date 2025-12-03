using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.GameTypes.ViewModels;

public record GameTypeViewModel
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public GameTypeViewModel(GameType entity)
    {
        Id = entity.Id;
        Name = entity.Name;
    }
}
