using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

/// <summary>
/// Implementation of the repository for the GameType entity.
/// </summary>
public class GameTypeRepository : Repository<GameType>, IGameTypeRepository
{
    private readonly ICurrentUserService _currentUserService;

    public GameTypeRepository(ApplicationDbContext context, ICurrentUserService currentUserService, IEntityValidationService entityValidationService) 
        : base(context, currentUserService, entityValidationService)
    {
        _currentUserService = currentUserService;
    }

    public async Task<GameType?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalizedName = name?.Trim() ?? string.Empty;
        
        return await DbSet
            .Where(gt => gt.UserId == _currentUserService.UserId && gt.Name == normalizedName)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<GameType>> GetOrCreateByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default)
    {
        var normalizedNames = names
            .Select(n => n?.Trim() ?? string.Empty)
            .Where(n => !string.IsNullOrEmpty(n))
            .Distinct()
            .ToList();

        if (normalizedNames.Count == 0)
        {
            return [];
        }

        // Get existing game types for this user
        var existingTypes = await DbSet
            .Where(gt => gt.UserId == _currentUserService.UserId && normalizedNames.Contains(gt.Name))
            .ToListAsync(cancellationToken);

        var existingNames = existingTypes.Select(gt => gt.Name).ToHashSet();
        var result = new List<GameType>(existingTypes);

        // Create missing game types
        foreach (var name in normalizedNames.Where(n => !existingNames.Contains(n)))
        {
            var newGameType = new GameType(name);
            await AddAsync(newGameType, cancellationToken);
            result.Add(newGameType);
        }

        return result;
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalizedName = name?.Trim() ?? string.Empty;
        
        return await DbSet
            .AnyAsync(gt => gt.UserId == _currentUserService.UserId && gt.Name == normalizedName, cancellationToken);
    }
}
