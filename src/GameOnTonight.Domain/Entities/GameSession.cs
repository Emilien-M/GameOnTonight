using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Entities;

/// <summary>
/// Represents a played game session.
/// </summary>
public class GameSession : UserOwnedEntity
{
    /// <summary>
    /// ID of the game to which this session refers.
    /// </summary>
    public int BoardGameId { get; set; }
    
    /// <summary>
    /// Reference to the board game.
    /// </summary>
    public virtual BoardGame? BoardGame { get; set; }
    
    /// <summary>
    /// Date the game was played.
    /// </summary>
    public DateTime PlayedAt { get; set; }
    
    /// <summary>
    /// Number of players who participated in this game.
    /// </summary>
    public int PlayerCount { get; set; }
    
    /// <summary>
    /// Notes or comments about the game (optional).
    /// </summary>
    public string? Notes { get; set; }
}
