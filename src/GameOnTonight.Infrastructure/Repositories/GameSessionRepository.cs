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
    public GameSessionRepository(ApplicationDbContext context, ICurrentUserService currentUserService) 
        : base(context, currentUserService)
    {
    }

    public async Task<IEnumerable<GameSession>> GetSessionHistoryAsync(string userId, int? count = null)
    {
        IQueryable<GameSession> query = DbSet
            .Where(s => s.UserId == userId)
            .Include(s => s.BoardGame)
            .OrderByDescending(s => s.PlayedAt);

        if (count.HasValue)
        {
            query = query.Take(count.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<GameSession>> GetSessionsByGameAsync(int boardGameId, string userId)
    {
        return await DbSet
            .Where(s => s.BoardGameId == boardGameId && s.UserId == userId)
            .OrderByDescending(s => s.PlayedAt)
            .ToListAsync();
    }

    public async Task<IDictionary<int, int>> GetGamePlayCountsAsync(string userId)
    {
        return await DbSet
            .Where(s => s.UserId == userId)
            .GroupBy(s => s.BoardGameId)
            .Select(g => new { BoardGameId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.BoardGameId, x => x.Count);
    }
}
