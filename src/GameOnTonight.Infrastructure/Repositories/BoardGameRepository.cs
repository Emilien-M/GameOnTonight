using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

/// <summary>
/// Implementation of the repository for the BoardGame entity.
/// </summary>
public class BoardGameRepository : Repository<BoardGame>, IBoardGameRepository
{
    private readonly ICurrentUserService _currentUserService;

    public BoardGameRepository(ApplicationDbContext context, ICurrentUserService currentUserService) 
        : base(context, currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public new async Task<BoardGame?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet
            .Include(g => g.GameTypes)
            .FirstOrDefaultAsync(g => g.Id == (int)id && g.UserId == _currentUserService.UserId, cancellationToken);
        
        return entity;
    }

    public new async Task<IEnumerable<BoardGame>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(g => g.GameTypes)
            .Where(g => g.UserId == _currentUserService.UserId)
            .ToListAsync(cancellationToken);
    }

    public new async Task<(IEnumerable<BoardGame> Items, int TotalCount)> GetAllPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = DbSet
            .Include(g => g.GameTypes)
            .Where(g => g.UserId == _currentUserService.UserId);
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return (items, totalCount);
    }

    public async Task<IEnumerable<BoardGame>> FilterGamesAsync(int playerCount, int maxDuration, IReadOnlyList<string>? gameTypes, CancellationToken cancellationToken = default)
    {
        // Commence par le filtre de base sur le nombre de joueurs et la durée
        var query = DbSet
            .Include(g => g.GameTypes)
            .Where(g => 
                g.UserId == _currentUserService.UserId &&
                g.MinPlayers <= playerCount && 
                g.MaxPlayers >= playerCount && 
                g.DurationMinutes <= maxDuration);

        // Ajoute le filtre sur les types de jeu si spécifié (logique OR)
        if (gameTypes is { Count: > 0 })
        {
            query = query.Where(g => g.GameTypes.Any(gt => gameTypes.Contains(gt.Name)));
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<BoardGame?> GetRandomGameAsync(IEnumerable<int> gameIds, CancellationToken cancellationToken = default)
    {
        var idList = gameIds.ToList();
        if (!idList.Any())
        {
            return null;
        }

        // Récupère tous les jeux correspondant aux IDs spécifiés et appartenant à l'utilisateur
        var games = await DbSet
            .Include(g => g.GameTypes)
            .Where(g => idList.Contains(g.Id) && g.UserId == _currentUserService.UserId)
            .ToListAsync(cancellationToken);

        if (!games.Any())
        {
            return null;
        }

        // Sélection aléatoire d'un jeu dans la liste
        var randomIndex = Random.Shared.Next(games.Count);
        return games[randomIndex];
    }

    public async Task<IEnumerable<string>> GetDistinctGameTypesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<GameType>()
            .Where(gt => gt.UserId == _currentUserService.UserId)
            .Select(gt => gt.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync(cancellationToken);
    }
}
