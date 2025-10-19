namespace GameOnTonight.Domain.Entities.Common;

/// <summary>
/// Interface for entities owned by a specific user.
/// </summary>
public interface IUserOwnedEntity
{
    /// <summary>
    /// Identifier of the user who owns the entity.
    /// </summary>
    string UserId { get; }
    
    /// <summary>
    /// Sets the identifier of the user who owns the entity, only before persistence.
    /// </summary>
    /// <param name="userId"></param>
    void SetUserId(string userId);
}
