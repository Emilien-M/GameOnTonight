using Microsoft.AspNetCore.Identity;

namespace GameOnTonight.Domain.Entities.Common;

/// <summary>
/// Base entity owned by a user, combining BaseEntity and IUserOwnedEntity.
/// </summary>
public abstract class UserOwnedEntity : BaseEntity, IUserOwnedEntity
{
    /// <summary>
    /// Identifier of the user who owns the entity.
    /// </summary>
    public string UserId { get; private set; } = string.Empty;
    public virtual IdentityUser User { get; private set; }

    public void SetUserId(string userId)
    {
        if (Id != 0)
        {
            throw new InvalidOperationException("The user ID can only be set before the entity is persisted.");
        }
        
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new ArgumentException("The user ID cannot be empty.", nameof(userId));
        }
        
        UserId = userId;
    }
}
