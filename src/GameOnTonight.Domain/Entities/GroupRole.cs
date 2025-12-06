namespace GameOnTonight.Domain.Entities;

/// <summary>
/// Defines the role of a member within a group.
/// </summary>
public enum GroupRole
{
    /// <summary>
    /// Creator and administrator of the group. Can delete the group, manage members,
    /// create invite codes, and manage all aspects of shared data.
    /// </summary>
    Owner = 0,

    /// <summary>
    /// Can view and contribute to shared data.
    /// </summary>
    Member = 1
}
