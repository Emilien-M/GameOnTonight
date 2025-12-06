namespace GameOnTonight.Domain.Exceptions;

/// <summary>
/// Centralized domain errors for group-related operations.
/// </summary>
public static class GroupErrors
{
    public static DomainError NameRequired => 
        new("The group name is required.", "Name");
    
    public static DomainError NameTooLong(int maxLength) => 
        new($"The group name cannot exceed {maxLength} characters.", "Name");
    
    public static DomainError UserAlreadyMember(string userId) => 
        new($"User '{userId}' is already a member of this group.", "Members");
    
    public static DomainError UserNotMember(string userId) => 
        new($"User '{userId}' is not a member of this group.", "Members");
    
    public static DomainError CannotRemoveOwner => 
        new("Cannot remove the owner. Transfer ownership first.", "Members");
    
    public static DomainError InsufficientPermissions(string action) => 
        new($"Insufficient permissions to {action}.", "Permissions");
    
    public static DomainError InvalidInviteCode => 
        new("The invite code is invalid or expired.", "InviteCode");
    
    public static DomainError InviteCodeExpired => 
        new("The invite code has expired.", "InviteCode");
    
    public static DomainError NewOwnerMustBeMember => 
        new("New owner must be a member of the group.", "Members");
}
