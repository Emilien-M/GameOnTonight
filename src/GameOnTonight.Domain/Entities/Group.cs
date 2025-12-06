using GameOnTonight.Domain.Entities.Common;
using GameOnTonight.Domain.Exceptions;

namespace GameOnTonight.Domain.Entities;

/// <summary>
/// Represents a group of users who share board games and game sessions.
/// </summary>
public class Group : BaseEntity
{
    private const int NameMaxLength = 100;
    
    private List<GroupMember> _members = [];
    private List<GroupInviteCode> _inviteCodes = [];

    /// <summary>
    /// Required by EF Core for materialization.
    /// </summary>
    private Group() { }

    /// <summary>
    /// Creates a new Group with validated properties.
    /// </summary>
    public Group(string name, string ownerUserId, DateTime now, string? description = null)
    {
        SetName(name);
        Description = description;
        
        // Add the creator as Owner
        _members.Add(new GroupMember(ownerUserId, GroupRole.Owner, now));
        
        // Create an initial invite code
        _inviteCodes.Add(new GroupInviteCode(ownerUserId, now));
        
        ThrowIfInvalid();
    }

    /// <summary>
    /// Name of the group.
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Optional description of the group.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Members of the group.
    /// </summary>
    public virtual IReadOnlyCollection<GroupMember> Members => _members.AsReadOnly();

    /// <summary>
    /// Active invite codes for this group.
    /// </summary>
    public virtual IReadOnlyCollection<GroupInviteCode> InviteCodes => _inviteCodes.AsReadOnly();

    /// <summary>
    /// Sets the group name with validation.
    /// </summary>
    public void SetName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            AddDomainError(GroupErrors.NameRequired);
            return;
        }
        
        if (name.Length > NameMaxLength)
        {
            AddDomainError(GroupErrors.NameTooLong(NameMaxLength));
            return;
        }
        
        Name = name.Trim();
    }

    /// <summary>
    /// Sets the group description.
    /// </summary>
    public void SetDescription(string? description)
    {
        Description = description?.Trim();
    }

    /// <summary>
    /// Creates a new invite code for this group.
    /// Only Owner can create invite codes.
    /// Returns null if validation fails (check HasErrors).
    /// </summary>
    public GroupInviteCode? CreateInviteCode(string createdByUserId, DateTime now)
    {
        var role = GetUserRole(createdByUserId);
        if (role != GroupRole.Owner)
        {
            AddDomainError(GroupErrors.InsufficientPermissions("create invite codes"));
            return null;
        }

        var inviteCode = new GroupInviteCode(createdByUserId, now);
        _inviteCodes.Add(inviteCode);
        return inviteCode;
    }

    /// <summary>
    /// Revokes an invite code.
    /// Only Owner can revoke invite codes.
    /// </summary>
    public void RevokeInviteCode(int inviteCodeId, string revokedByUserId)
    {
        var role = GetUserRole(revokedByUserId);
        if (role != GroupRole.Owner)
        {
            AddDomainError(GroupErrors.InsufficientPermissions("revoke invite codes"));
            return;
        }

        var code = _inviteCodes.FirstOrDefault(c => c.Id == inviteCodeId);
        if (code != null)
        {
            _inviteCodes.Remove(code);
        }
    }

    /// <summary>
    /// Adds a new member to the group.
    /// Returns null if validation fails (check HasErrors).
    /// </summary>
    public GroupMember? AddMember(string userId, DateTime now)
    {
        if (_members.Any(m => m.UserId == userId))
        {
            AddDomainError(GroupErrors.UserAlreadyMember(userId));
            return null;
        }

        var member = new GroupMember(userId, GroupRole.Member, now);
        _members.Add(member);
        return member;
    }

    /// <summary>
    /// Removes a member from the group.
    /// </summary>
    public void RemoveMember(string userId)
    {
        var member = _members.FirstOrDefault(m => m.UserId == userId);
        if (member == null)
        {
            AddDomainError(GroupErrors.UserNotMember(userId));
            return;
        }

        if (member.Role == GroupRole.Owner)
        {
            AddDomainError(GroupErrors.CannotRemoveOwner);
            return;
        }

        _members.Remove(member);
    }

    /// <summary>
    /// Transfers ownership to another member.
    /// </summary>
    public void TransferOwnership(string currentOwnerId, string newOwnerId)
    {
        var currentOwner = _members.FirstOrDefault(m => m.UserId == currentOwnerId);
        var newOwner = _members.FirstOrDefault(m => m.UserId == newOwnerId);

        if (currentOwner?.Role != GroupRole.Owner)
        {
            AddDomainError(GroupErrors.InsufficientPermissions("transfer ownership"));
            return;
        }

        if (newOwner == null)
        {
            AddDomainError(GroupErrors.NewOwnerMustBeMember);
            return;
        }

        currentOwner.UpdateRole(GroupRole.Member);
        newOwner.UpdateRole(GroupRole.Owner);
    }

    /// <summary>
    /// Gets the role of a user in this group.
    /// </summary>
    public GroupRole? GetUserRole(string userId)
    {
        return _members.FirstOrDefault(m => m.UserId == userId)?.Role;
    }

    /// <summary>
    /// Checks if a user is a member of this group.
    /// </summary>
    public bool IsMember(string userId)
    {
        return _members.Any(m => m.UserId == userId);
    }

    /// <summary>
    /// Checks if a user is the owner of this group.
    /// </summary>
    public bool IsOwner(string userId)
    {
        return GetUserRole(userId) == GroupRole.Owner;
    }
}
