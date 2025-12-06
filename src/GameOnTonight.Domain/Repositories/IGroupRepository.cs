using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Domain.Repositories;

/// <summary>
/// Repository interface for Group entities.
/// </summary>
public interface IGroupRepository : IRepository<Group>
{
    /// <summary>
    /// Gets a group by its ID with members loaded.
    /// </summary>
    Task<Group?> GetByIdWithMembersAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a group by an invite code.
    /// </summary>
    Task<Group?> GetByInviteCodeAsync(string inviteCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all groups that a user is a member of.
    /// </summary>
    Task<IEnumerable<Group>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the IDs of all groups that a user is a member of.
    /// </summary>
    Task<IEnumerable<int>> GetUserGroupIdsAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a user is a member of a group.
    /// </summary>
    Task<bool> IsUserMemberAsync(int groupId, string userId, CancellationToken cancellationToken = default);
}
