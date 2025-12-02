using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Persistence;
using GameOnTonight.Infrastructure.Services;
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

    public async Task<IEnumerable<BoardGame>> FilterGamesAsync(int playerCount, int maxDuration, string? gameType, CancellationToken cancellationToken = default)
    {
        // Commence par le filtre de base sur le nombre de joueurs et la durée
        var query = DbSet.Where(g => 
            g.UserId == _currentUserService.UserId &&
            g.MinPlayers <= playerCount && 
            g.MaxPlayers >= playerCount && 
            g.DurationMinutes <= maxDuration);

        // Ajoute le filtre sur le type de jeu si spécifié
        if (!string.IsNullOrEmpty(gameType))
        {
            query = query.Where(g => g.GameType == gameType);
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
        return await DbSet
            .Where(g => g.UserId == _currentUserService.UserId)
            .Select(g => g.GameType)
            .Distinct()
            .OrderBy(type => type)
            .ToListAsync(cancellationToken);
    }
}
