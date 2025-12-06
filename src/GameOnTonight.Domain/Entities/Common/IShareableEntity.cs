namespace GameOnTonight.Domain.Entities.Common;

/// <summary>
/// Interface for entities that can be shared with a group.
/// </summary>
public interface IShareableEntity : IUserOwnedEntity
{
    /// <summary>
    /// Identifier of the group this entity is shared with.
    /// Null means the entity is private to the owner.
    /// </summary>
    int? GroupId { get; }

    /// <summary>
    /// Sets the group this entity is shared with.
    /// </summary>
    /// <param name="groupId">The group ID, or null to make private.</param>
    void SetGroupId(int? groupId);
}
