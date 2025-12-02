using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Entities;

/// <summary>
/// Represents a player who participated in a game session.
/// </summary>
public class GameSessionPlayer : BaseEntity
{
    /// <summary>
    /// ID of the game session.
    /// </summary>
    public int GameSessionId { get; set; }

    /// <summary>
    /// Reference to the game session.
    /// </summary>
    public virtual GameSession? GameSession { get; set; }

    /// <summary>
    /// Name of the player.
    /// </summary>
    public string PlayerName { get; private set; } = string.Empty;

    /// <summary>
    /// Score achieved by the player (optional, depends on game type).
    /// </summary>
    public int? Score { get; private set; }

    /// <summary>
    /// Indicates whether this player won the game.
    /// </summary>
    public bool IsWinner { get; private set; }

    /// <summary>
    /// Order/position of the player (1st, 2nd, 3rd, etc.) if applicable.
    /// </summary>
    public int? Position { get; private set; }

    // Private constructor for EF Core
    private GameSessionPlayer() { }

    public GameSessionPlayer(string playerName, int? score = null, bool isWinner = false, int? position = null)
    {
        SetPlayerName(playerName);
        SetScore(score);
        IsWinner = isWinner;
        Position = position;
    }

    public void SetPlayerName(string name)
    {
        ValidateString(name, nameof(PlayerName), 100);
        if (!HasErrors)
        {
            PlayerName = name.Trim();
        }
    }

    public void SetScore(int? score)
    {
        if (score.HasValue)
        {
            ValidateNumber(score.Value, nameof(Score), min: 0);
        }
        if (!HasErrors)
        {
            Score = score;
        }
    }

    public void SetAsWinner(bool isWinner)
    {
        IsWinner = isWinner;
    }

    public void SetPosition(int? position)
    {
        if (position.HasValue)
        {
            ValidateNumber(position.Value, nameof(Position), min: 1);
        }
        if (!HasErrors)
        {
            Position = position;
        }
    }
}
