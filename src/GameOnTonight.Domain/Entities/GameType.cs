using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Entities;

/// <summary>
/// Represents a game type/category in a user's collection.
/// </summary>
public class GameType : UserOwnedEntity
{
    /// <summary>
    /// Required by EF Core for materialization.
    /// </summary>
    private GameType() { }

    /// <summary>
    /// Creates a new GameType with validated properties.
    /// </summary>
    /// <param name="name">The name of the game type.</param>
    public GameType(string name)
    {
        SetName(name);
        ThrowIfInvalid();
    }

    /// <summary>
    /// Name of the game type.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Board games associated with this type.
    /// </summary>
    public virtual ICollection<BoardGame> BoardGames { get; private set; } = new List<BoardGame>();

    /// <summary>
    /// Updates the game type name with validation.
    /// </summary>
    /// <param name="name">The new name.</param>
    public void Update(string name)
    {
        ClearDomainErrors();
        SetName(name);
        ThrowIfInvalid();
    }

    private void SetName(string name)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        ValidateString(trimmedName, nameof(Name), 100);
        Name = trimmedName;
    }
}
