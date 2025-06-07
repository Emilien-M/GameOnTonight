using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Entities;

/// <summary>
/// Classe représentant un jeu dans la collection d'un utilisateur
/// </summary>
public class BoardGame : UserOwnedEntity
{
    /// <summary>
    /// Nom du jeu
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Nombre minimum de joueurs requis pour jouer
    /// </summary>
    public int MinPlayers { get; set; }
    
    /// <summary>
    /// Nombre maximum de joueurs pouvant participer
    /// </summary>
    public int MaxPlayers { get; set; }
    
    /// <summary>
    /// Durée approximative d'une partie en minutes
    /// </summary>
    public int DurationMinutes { get; set; }
    
    /// <summary>
    /// Type ou catégorie du jeu
    /// </summary>
    public string GameType { get; set; } = string.Empty;
    
    /// <summary>
    /// Description optionnelle du jeu
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// URL de l'image de la boîte de jeu (optionnel)
    /// </summary>
    public string? ImageUrl { get; set; }
    
    /// <summary>
    /// Liste des parties enregistrées avec ce jeu
    /// </summary>
    public virtual ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
}
