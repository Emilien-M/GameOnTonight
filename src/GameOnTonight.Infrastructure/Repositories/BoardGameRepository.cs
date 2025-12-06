using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

/// <summary>
/// Implementation of the repository for the BoardGame entity.
/// Inherits from ShareableRepository to include games shared via groups.
/// </summary>
public class BoardGameRepository : ShareableRepository<BoardGame>, IBoardGameRepository
{
    public BoardGameRepository(
        ApplicationDbContext context, 
        ICurrentUserService currentUserService,
        IGroupRepository groupRepository,
        IEntityValidationService entityValidationService) 
        : base(context, currentUserService, groupRepository, entityValidationService)
    {
    }

    public new async Task<BoardGame?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        var query = await GetShareableQueryAsync(cancellationToken);
        
        return await query
            .Include(g => g.GameTypes)
            .Include(g => g.Group)
            .FirstOrDefaultAsync(g => g.Id == (int)id, cancellationToken);
    }

    public new async Task<IEnumerable<BoardGame>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var query = await GetShareableQueryAsync(cancellationToken);
        
        return await query
            .Include(g => g.GameTypes)
            .Include(g => g.Group)
            .ToListAsync(cancellationToken);
    }

    public new async Task<(IEnumerable<BoardGame> Items, int TotalCount)> GetAllPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = await GetShareableQueryAsync(cancellationToken);
        query = query.Include(g => g.GameTypes);
        
        var totalCount = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return (items, totalCount);
    }

    public async Task<IEnumerable<BoardGame>> FilterGamesAsync(int playerCount, int maxDuration, IReadOnlyList<string>? gameTypes, CancellationToken cancellationToken = default)
    {
        var baseQuery = await GetShareableQueryAsync(cancellationToken);
        
        // Commence par le filtre de base sur le nombre de joueurs et la durée
        var query = baseQuery
            .Include(g => g.GameTypes)
            .Where(g => 
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

        var baseQuery = await GetShareableQueryAsync(cancellationToken);
        
        // Récupère tous les jeux correspondant aux IDs spécifiés et accessibles par l'utilisateur
        var games = await baseQuery
            .Include(g => g.GameTypes)
            .Where(g => idList.Contains(g.Id))
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
        var baseQuery = await GetShareableQueryAsync(cancellationToken);
        
        // Récupère les types de jeux des jeux accessibles par l'utilisateur (possédés + partagés)
        return await baseQuery
            .Include(g => g.GameTypes)
            .SelectMany(g => g.GameTypes)
            .Select(gt => gt.Name)
            .Distinct()
            .OrderBy(name => name)
            .ToListAsync(cancellationToken);
    }
}
