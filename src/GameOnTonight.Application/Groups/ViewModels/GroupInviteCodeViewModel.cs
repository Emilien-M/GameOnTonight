using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Application.Groups.ViewModels;

/// <summary>
/// View model for an invite code.
/// </summary>
public sealed class GroupInviteCodeViewModel
{
    public GroupInviteCodeViewModel(GroupInviteCode inviteCode)
    {
        Id = inviteCode.Id;
        Code = inviteCode.Code;
        ExpiresAt = inviteCode.ExpiresAt;
        CreatedAt = inviteCode.CreatedAt;
    }

    /// <summary>
    /// Invite code identifier.
    /// </summary>
    public int Id { get; init; }

    /// <summary>
    /// The invite code string (16 characters).
    /// </summary>
    public string Code { get; init; }

    /// <summary>
    /// Expiration date.
    /// </summary>
    public DateTime ExpiresAt { get; init; }

    /// <summary>
    /// Creation date.
    /// </summary>
    public DateTime CreatedAt { get; init; }
}
