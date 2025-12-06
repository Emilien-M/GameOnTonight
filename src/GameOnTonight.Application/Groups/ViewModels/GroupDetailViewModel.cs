using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.Groups.ViewModels;

/// <summary>
/// Detailed view model for a group including members and invite codes.
/// </summary>
public sealed class GroupDetailViewModel
{
    public GroupDetailViewModel(Group group, string currentUserId, DateTime now)
    {
        Id = group.Id;
        Name = group.Name;
        Description = group.Description;
        CreatedAt = group.CreatedAt;
        
        var userMember = group.Members.FirstOrDefault(m => m.UserId == currentUserId);
        CurrentUserRole = userMember?.Role;
        IsOwner = userMember?.Role == GroupRole.Owner;
        
        Members = group.Members
            .OrderByDescending(m => m.Role)
            .ThenBy(m => m.JoinedAt)
            .Select(m => new GroupMemberViewModel(m))
            .ToList();

        // Only show invite codes to the owner
        if (IsOwner)
        {
            InviteCodes = group.InviteCodes
                .Where(c => c.IsValid(now))
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new GroupInviteCodeViewModel(c))
                .ToList();
        }
        else
        {
            InviteCodes = new List<GroupInviteCodeViewModel>();
        }
    }

    public int Id { get; init; }
    public string Name { get; init; }
    public string? Description { get; init; }
    public DateTime CreatedAt { get; init; }
    public GroupRole? CurrentUserRole { get; init; }
    public bool IsOwner { get; init; }
    public IReadOnlyList<GroupMemberViewModel> Members { get; init; }
    public IReadOnlyList<GroupInviteCodeViewModel> InviteCodes { get; init; }
}
