using GameOnTonight.Domain.Entities.Common;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

/// <summary>
/// Repository for shareable entities that includes group-shared items.
/// </summary>
/// <typeparam name="TEntity">Type of the shareable entity.</typeparam>
public class ShareableRepository<TEntity> : Repository<TEntity> 
    where TEntity : BaseEntity, IShareableEntity
{
    private readonly IGroupRepository _groupRepository;
    private IEnumerable<int>? _cachedUserGroupIds;

    public ShareableRepository(
        ApplicationDbContext context,
        ICurrentUserService currentUserService,
        IGroupRepository groupRepository, 
        IEntityValidationService entityValidationService) 
        : base(context, currentUserService, entityValidationService)
    {
        _groupRepository = groupRepository;
    }

    /// <summary>
    /// Gets the base query filtered by user ownership or group membership.
    /// </summary>
    protected async Task<IQueryable<TEntity>> GetShareableQueryAsync(CancellationToken cancellationToken = default)
    {
        var userId = CurrentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        var groupIds = await GetUserGroupIdsAsync(userId, cancellationToken);

        return DbSet.Where(e => 
            e.UserId == userId || 
            (e.GroupId != null && groupIds.Contains(e.GroupId.Value)));
    }

    /// <summary>
    /// Gets only user's own items (not shared with any group or explicitly owned).
    /// </summary>
    public async Task<IEnumerable<TEntity>> GetOwnedItemsAsync(CancellationToken cancellationToken = default)
    {
        var userId = CurrentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        return await DbSet
            .Where(e => e.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets only private items (owned by user and not shared with any group).
    /// </summary>
    public async Task<IEnumerable<TEntity>> GetPrivateItemsAsync(CancellationToken cancellationToken = default)
    {
        var userId = CurrentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        return await DbSet
            .Where(e => e.UserId == userId && e.GroupId == null)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets items shared with a specific group.
    /// </summary>
    public async Task<IEnumerable<TEntity>> GetByGroupIdAsync(int groupId, CancellationToken cancellationToken = default)
    {
        var userId = CurrentUserService.UserId 
            ?? throw new UnauthorizedAccessException("User not authenticated");

        // Verify user is member of the group
        var isMember = await _groupRepository.IsUserMemberAsync(groupId, userId, cancellationToken);
        if (!isMember)
            return Enumerable.Empty<TEntity>();

        return await DbSet
            .Where(e => e.GroupId == groupId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Gets cached user group IDs to avoid repeated queries.
    /// </summary>
    protected async Task<List<int>> GetUserGroupIdsAsync(string userId, CancellationToken cancellationToken)
    {
        if (_cachedUserGroupIds != null)
            return _cachedUserGroupIds.ToList();

        _cachedUserGroupIds = await _groupRepository.GetUserGroupIdsAsync(userId, cancellationToken);
        return _cachedUserGroupIds.ToList();
    }

    /// <summary>
    /// Gets the group repository for subclasses.
    /// </summary>
    protected IGroupRepository GroupRepository => _groupRepository;
}
