using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Group entities.
/// </summary>
public class GroupRepository : Repository<Group>, IGroupRepository
{
    private readonly ApplicationDbContext _context;

    public GroupRepository(ApplicationDbContext context, ICurrentUserService currentUserService, IEntityValidationService entityValidationService) 
        : base(context, currentUserService, entityValidationService)
    {
        _context = context;
    }

    public async Task<Group?> GetByIdWithMembersAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Include(g => g.Members).ThenInclude(m => m.Profile)
            .Include(g => g.InviteCodes)
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<Group?> GetByInviteCodeAsync(string inviteCode, CancellationToken cancellationToken = default)
    {
        return await _context.GroupInviteCodes
            .Include(c => c.Group)
                .ThenInclude(g => g.Members)
            .Where(c => c.Code == inviteCode && c.ExpiresAt > DateTime.UtcNow)
            .Select(c => c.Group)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Group>> GetUserGroupsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Groups
            .Include(g => g.Members)
            .Where(g => g.Members.Any(m => m.UserId == userId))
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<int>> GetUserGroupIdsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupMembers
            .Where(m => m.UserId == userId)
            .Select(m => m.GroupId)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsUserMemberAsync(int groupId, string userId, CancellationToken cancellationToken = default)
    {
        return await _context.GroupMembers
            .AnyAsync(m => m.GroupId == groupId && m.UserId == userId, cancellationToken);
    }
}
