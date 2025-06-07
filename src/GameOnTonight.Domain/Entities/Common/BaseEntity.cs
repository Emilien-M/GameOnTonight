namespace GameOnTonight.Domain.Entities.Common;

/// <summary>
/// Entité de base avec les propriétés communes
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Identifiant unique de l'entité
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Date de création de l'entité
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de dernière mise à jour de l'entité
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
