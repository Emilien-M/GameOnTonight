using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.Groups.ViewModels;

/// <summary>
/// View model for a group member.
/// </summary>
public sealed class GroupMemberViewModel
{
    public GroupMemberViewModel(GroupMember member)
    {
        Id = member.Id;
        UserId = member.UserId;
        DisplayName = member.Profile?.DisplayName ?? "Unknown";
        Role = member.Role;
        JoinedAt = member.JoinedAt;
    }

    /// <summary>
    /// Member identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// User identifier.
    /// </summary>
    public string UserId { get; init; }
    
    /// <summary>
    /// Display name from the user's profile.
    /// </summary>
    public string DisplayName { get; init; }

    /// <summary>
    /// Role in the group.
    /// </summary>
    public GroupRole Role { get; init; }

    /// <summary>
    /// Date when the user joined the group.
    /// </summary>
    public DateTime JoinedAt { get; init; }
}
