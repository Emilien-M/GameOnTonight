using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Entities;

/// <summary>
/// Represents a game in a user's collection.
/// </summary>
public class BoardGame : UserOwnedEntity
{
    /// <summary>
    /// Name of the game.
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Minimum number of players required to play.
    /// </summary>
    public int MinPlayers { get; set; }
    
    /// <summary>
    /// Maximum number of players that can participate.
    /// </summary>
    public int MaxPlayers { get; set; }
    
    /// <summary>
    /// Approximate duration of a game in minutes.
    /// </summary>
    public int DurationMinutes { get; set; }
    
    /// <summary>
    /// Type or category of the game.
    /// </summary>
    public string GameType { get; set; } = string.Empty;
    
    /// <summary>
    /// Optional description of the game.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// URL of the game box image (optional).
    /// </summary>
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// List of recorded games with this game.
    /// </summary>
    public virtual ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
}
