using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.Groups.ViewModels;

/// <summary>
/// View model for a group.
/// </summary>
public sealed class GroupViewModel
{
    public GroupViewModel(Group group, string currentUserId)
    {
        Id = group.Id;
        Name = group.Name;
        Description = group.Description;
        CreatedAt = group.CreatedAt;
        MemberCount = group.Members.Count;
        
        var userMember = group.Members.FirstOrDefault(m => m.UserId == currentUserId);
        CurrentUserRole = userMember?.Role;
        IsOwner = userMember?.Role == GroupRole.Owner;
    }

    /// <summary>
    /// Group identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// Name of the group.
    /// </summary>
    public string Name { get; init; }

    /// <summary>
    /// Optional description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Date when the group was created.
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// Number of members in the group.
    /// </summary>
    public int MemberCount { get; init; }

    /// <summary>
    /// Current user's role in the group.
    /// </summary>
    public GroupRole? CurrentUserRole { get; init; }

    /// <summary>
    /// Whether the current user is the owner.
    /// </summary>
    public bool IsOwner { get; init; }
}
