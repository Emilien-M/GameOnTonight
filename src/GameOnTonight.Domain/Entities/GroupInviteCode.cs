using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Entities;

/// <summary>
/// Represents an invite code for joining a group.
/// </summary>
public class GroupInviteCode : BaseEntity
{
    private const int CodeLength = 16;
    private const int ExpirationDays = 7;
    private const string ValidCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

    /// <summary>
    /// Required by EF Core for materialization.
    /// </summary>
    private GroupInviteCode() { }

    /// <summary>
    /// Creates a new invite code with automatic expiration.
    /// </summary>
    /// <param name="createdByUserId">The user ID who created this code.</param>
    internal GroupInviteCode(string createdByUserId, DateTime now)
    {
        Code = GenerateCode();
        ExpiresAt = now.AddDays(ExpirationDays);
        CreatedByUserId = createdByUserId;
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
    /// The invite code (16 alphanumeric characters).
    /// </summary>
    public string Code { get; private set; } = string.Empty;

    /// <summary>
    /// Expiration date of the invite code.
    /// </summary>
    public DateTime ExpiresAt { get; private set; }

    /// <summary>
    /// The user ID who created this invite code.
    /// </summary>
    public string CreatedByUserId { get; private set; } = string.Empty;

    /// <summary>
    /// Checks if the invite code is still valid (not expired).
    /// </summary>
    public bool IsValid(DateTime now)
    {
        return now < ExpiresAt;
    }

    /// <summary>
    /// Generates a random alphanumeric code.
    /// </summary>
    private static string GenerateCode()
    {
        var random = Random.Shared;
        var code = new char[CodeLength];
        
        for (var i = 0; i < CodeLength; i++)
        {
            code[i] = ValidCharacters[random.Next(ValidCharacters.Length)];
        }
        
        return new string(code);
    }
}
