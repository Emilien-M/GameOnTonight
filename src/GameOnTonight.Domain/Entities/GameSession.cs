using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Entities;

/// <summary>
/// Classe représentant une partie jouée
/// </summary>
public class GameSession : UserOwnedEntity
{
    /// <summary>
    /// ID du jeu auquel cette partie fait référence
    /// </summary>
    public int BoardGameId { get; set; }
    
    /// <summary>
    /// Référence au jeu de société
    /// </summary>
    public virtual BoardGame? BoardGame { get; set; }
    
    /// <summary>
    /// Date à laquelle la partie a été jouée
    /// </summary>
    public DateTime PlayedAt { get; set; }
    
    /// <summary>
    /// Nombre de joueurs ayant participé à cette partie
    /// </summary>
    public int PlayerCount { get; set; }
    
    /// <summary>
    /// Notes ou commentaires sur la partie (optionnel)
    /// </summary>
    public string? Notes { get; set; }
}
