using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Entities;

/// <summary>
/// Represents a game in a user's collection.
/// </summary>
public class BoardGame : UserOwnedEntity
{
    /// <summary>
    /// Required by EF Core for materialization.
    /// </summary>
    private BoardGame() { }

    /// <summary>
    /// Creates a new BoardGame with validated properties.
    /// </summary>
    public BoardGame(string name, int minPlayers, int maxPlayers, int durationMinutes, string gameType, string? description = null, string? imageUrl = null)
    {
        SetName(name);
        SetPlayerRange(minPlayers, maxPlayers);
        SetDuration(durationMinutes);
        SetGameType(gameType);
        Description = description;
        ImageUrl = imageUrl;
        
        ThrowIfInvalid();
    }

    /// <summary>
    /// Name of the game.
    /// </summary>
    public string Name { get; private set; } = string.Empty;
    
    /// <summary>
    /// Minimum number of players required to play.
    /// </summary>
    public int MinPlayers { get; private set; }
    
    /// <summary>
    /// Maximum number of players that can participate.
    /// </summary>
    public int MaxPlayers { get; private set; }
    
    /// <summary>
    /// Approximate duration of a game in minutes.
    /// </summary>
    public int DurationMinutes { get; private set; }
    
    /// <summary>
    /// Type or category of the game.
    /// </summary>
    public string GameType { get; private set; } = string.Empty;
    
    /// <summary>
    /// Optional description of the game.
    /// </summary>
    public string? Description { get; private set; }
    
    /// <summary>
    /// URL of the game box image (optional).
    /// </summary>
    public string? ImageUrl { get; private set; }
    
    /// <summary>
    /// List of recorded games with this game.
    /// </summary>
    public virtual ICollection<GameSession> GameSessions { get; private set; } = new List<GameSession>();

    /// <summary>
    /// Updates the board game properties with validation.
    /// </summary>
    public void Update(string name, int minPlayers, int maxPlayers, int durationMinutes, string gameType, string? description = null, string? imageUrl = null)
    {
        ClearDomainErrors();
        
        SetName(name);
        SetPlayerRange(minPlayers, maxPlayers);
        SetDuration(durationMinutes);
        SetGameType(gameType);
        Description = description;
        ImageUrl = imageUrl;
        
        ThrowIfInvalid();
    }

    private void SetName(string name)
    {
        var trimmedName = name?.Trim() ?? string.Empty;
        ValidateString(trimmedName, nameof(Name), 200);
        Name = trimmedName;
    }

    private void SetPlayerRange(int minPlayers, int maxPlayers)
    {
        ValidateNumber(minPlayers, nameof(MinPlayers), min: 1);
        ValidateNumber(maxPlayers, nameof(MaxPlayers), min: 1);
        
        if (minPlayers > maxPlayers)
        {
            AddDomainError(nameof(MaxPlayers), "MaxPlayers must be greater than or equal to MinPlayers.");
        }
        
        MinPlayers = minPlayers;
        MaxPlayers = maxPlayers;
    }

    private void SetDuration(int durationMinutes)
    {
        ValidateNumber(durationMinutes, nameof(DurationMinutes), min: 1);
        DurationMinutes = durationMinutes;
    }

    private void SetGameType(string gameType)
    {
        var trimmedGameType = gameType?.Trim() ?? string.Empty;
        ValidateString(trimmedGameType, nameof(GameType), 100);
        GameType = trimmedGameType;
    }
}
