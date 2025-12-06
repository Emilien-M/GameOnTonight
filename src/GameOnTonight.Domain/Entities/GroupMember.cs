using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Entities;

/// <summary>
/// Represents a user's membership in a group.
/// </summary>
public class GroupMember : BaseEntity
{
    private GroupMember() { }

    /// <summary>
    /// Creates a new GroupMember.
    /// </summary>
    internal GroupMember(string userId, GroupRole role, DateTime now)
    {
        UserId = userId;
        Role = role;
        JoinedAt = now;
    }

    /// <summary>
    /// Foreign key to the group.
    /// </summary>
    public int GroupId { get; private set; }

    /// <summary>
    /// Reference to the group.
    /// </summary>
    public virtual Group Group { get; private set; } = null!;

    /// <summary>
    /// The user ID (from Identity).
    /// </summary>
    public string UserId { get; private set; } = string.Empty;

    /// <summary>
    /// The user's role in the group.
    /// </summary>
    public GroupRole Role { get; private set; }

    /// <summary>
    /// Date when the user joined the group.
    /// </summary>
    public DateTime JoinedAt { get; private set; }

    /// <summary>
    /// Reference to the user's profile (for display purposes).
    /// </summary>
    public virtual Profile? Profile { get; private set; }

    /// <summary>
    /// Updates the member's role (internal use for ownership transfer only).
    /// </summary>
    internal void UpdateRole(GroupRole role)
    {
        Role = role;
    }
}
