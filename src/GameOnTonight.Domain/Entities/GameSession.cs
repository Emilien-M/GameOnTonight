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

    /// <summary>
    /// Personal rating of the session (1-5 stars).
    /// </summary>
    public int? Rating { get; private set; }

    /// <summary>
    /// URL of a photo from the game session (optional).
    /// </summary>
    public string? PhotoUrl { get; set; }

    /// <summary>
    /// Players who participated in this session with their scores.
    /// </summary>
    public virtual ICollection<GameSessionPlayer> Players { get; set; } = new List<GameSessionPlayer>();

    /// <summary>
    /// Sets the rating (1-5 stars) for this session.
    /// </summary>
    public void SetRating(int? rating)
    {
        if (rating.HasValue)
        {
            if (rating < 1 || rating > 5)
            {
                AddDomainError(nameof(Rating), "La note doit être comprise entre 1 et 5.");
                return;
            }
        }
        Rating = rating;
    }

    /// <summary>
    /// Adds a player to the session.
    /// </summary>
    public void AddPlayer(GameSessionPlayer player)
    {
        player.GameSession = this;
        Players.Add(player);
    }

    /// <summary>
    /// Clears all players from the session.
    /// </summary>
    public void ClearPlayers()
    {
        Players.Clear();
    }
}
