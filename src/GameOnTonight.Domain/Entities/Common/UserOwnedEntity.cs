namespace GameOnTonight.Domain.Entities.Common;

/// <summary>
/// Entité de base appartenant à un utilisateur, combinant BaseEntity et IUserOwnedEntity
/// </summary>
public abstract class UserOwnedEntity : BaseEntity, IUserOwnedEntity
{
    /// <summary>
    /// Identifiant de l'utilisateur propriétaire de l'entité
    /// </summary>
    public string UserId { get; private set; } = string.Empty;

    public void SetUserId(string userId)
    {
        if (Id != 0)
        {
            throw new InvalidOperationException("L'identifiant de l'utilisateur ne peut être défini qu'avant la persistance de l'entité.");
        }
        
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("L'identifiant de l'utilisateur ne peut pas être vide.", nameof(userId));
        }
        
        UserId = userId;
    }
}
