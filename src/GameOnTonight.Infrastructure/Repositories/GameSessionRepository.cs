using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

/// <summary>
/// Implementation of the repository for the GameSession entity.
/// </summary>
public class GameSessionRepository : Repository<GameSession>, IGameSessionRepository
{
    private readonly ICurrentUserService _currentUserService;

    public GameSessionRepository(ApplicationDbContext context, ICurrentUserService currentUserService, IEntityValidationService entityValidationService) 
        : base(context, currentUserService, entityValidationService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<IEnumerable<GameSession>> GetSessionHistoryAsync(int? count = null)
    {
        IQueryable<GameSession> query = DbSet
            .Where(s => s.UserId == _currentUserService.UserId)
            .Include(s => s.BoardGame)
            .Include(s => s.Players)
            .Include(s => s.Group)
            .OrderByDescending(s => s.PlayedAt);

        if (count.HasValue)
        {
            query = query.Take(count.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<GameSession>> GetSessionsByGameAsync(int boardGameId)
    {
        return await DbSet
            .Where(s => s.BoardGameId == boardGameId && s.UserId == _currentUserService.UserId)
            .OrderByDescending(s => s.PlayedAt)
            .ToListAsync();
    }

    public async Task<IDictionary<int, int>> GetGamePlayCountsAsync()
    {
        return await DbSet
            .Where(s => s.UserId == _currentUserService.UserId)
            .GroupBy(s => s.BoardGameId)
            .Select(g => new { BoardGameId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.BoardGameId, x => x.Count);
    }

    public async Task<GameSession?> GetByIdWithPlayersAsync(int id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(s => s.Id == id && s.UserId == _currentUserService.UserId)
            .Include(s => s.BoardGame)
            .Include(s => s.Players)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
