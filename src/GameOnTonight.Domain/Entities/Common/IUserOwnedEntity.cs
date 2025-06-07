namespace GameOnTonight.Domain.Entities.Common;

/// <summary>
/// Interface pour les entités appartenant à un utilisateur spécifique
/// </summary>
public interface IUserOwnedEntity
{
    /// <summary>
    /// Identifiant de l'utilisateur propriétaire de l'entité
    /// </summary>
    string UserId { get; }
    
    /// <summary>
    /// Définit l'identifiant de l'utilisateur propriétaire de l'entité, seulement avant la persistance
    /// </summary>
    /// <param name="userId"></param>
    void SetUserId(string userId);
}
