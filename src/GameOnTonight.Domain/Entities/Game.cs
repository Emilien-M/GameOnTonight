using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Entities;

/// <summary>
/// Entité représentant un jeu
/// </summary>
public class Game : UserOwnedEntity
{
    /// <summary>
    /// Nom du jeu
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Description du jeu
    /// </summary>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// Indique si le jeu est actif
    /// </summary>
    public bool IsActive { get; set; }
}
