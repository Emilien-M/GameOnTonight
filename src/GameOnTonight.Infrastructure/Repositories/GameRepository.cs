using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Infrastructure.Persistence;
using GameOnTonight.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

/// <summary>
/// Implémentation du repository pour l'entité Game
/// </summary>
public class GameRepository : Repository<Game>, IGameRepository
{
    public GameRepository(ApplicationDbContext context, ICurrentUserService currentUserService) 
        : base(context, currentUserService)
    {
    }

    public async Task<IEnumerable<Game>> GetActiveGamesAsync(string userId)
    {
        return await DbSet
            .Where(g => g.IsActive && g.UserId == userId)
            .ToListAsync();
    }
}
