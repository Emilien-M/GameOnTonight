namespace GameOnTonight.Domain.Entities.Common;

/// <summary>
/// Base entity that can be owned by a user and optionally shared with a group.
/// </summary>
public abstract class ShareableUserOwnedEntity : UserOwnedEntity, IShareableEntity
{
    /// <summary>
    /// Identifier of the group this entity is shared with.
    /// Null means the entity is private to the owner.
    /// </summary>
    public int? GroupId { get; private set; }

    /// <summary>
    /// Navigation property to the group.
    /// </summary>
    public virtual Group? Group { get; private set; }

    /// <summary>
    /// Sets the group this entity is shared with.
    /// </summary>
    /// <param name="groupId">The group ID, or null to make private.</param>
    public void SetGroupId(int? groupId)
    {
        GroupId = groupId;
    }

    /// <summary>
    /// Shares this entity with a group.
    /// </summary>
    /// <param name="groupId">The group ID to share with.</param>
    public void ShareWithGroup(int groupId)
    {
        SetGroupId(groupId);
    }

    /// <summary>
    /// Makes this entity private (removes from group).
    /// </summary>
    public void MakePrivate()
    {
        SetGroupId(null);
    }
}
