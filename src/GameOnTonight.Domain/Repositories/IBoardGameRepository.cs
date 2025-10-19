using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Domain.Repositories;

/// <summary>
/// Repository interface for the BoardGame entity.
/// </summary>
public interface IBoardGameRepository : IRepository<BoardGame>
{
    /// <summary>
    /// Filters games according to the specified criteria.
    /// </summary>
    /// <param name="playerCount">Number of players.</param>
    /// <param name="maxDuration">Maximum duration in minutes.</param>
    /// <param name="gameType">Type of game (optional).</param>
    /// <param name="userId">ID of the owning user.</param>
    /// <returns>List of games matching the criteria.</returns>
    Task<IEnumerable<BoardGame>> FilterGamesAsync(int playerCount, int maxDuration, string? gameType, string userId);
    
    /// <summary>
    /// Retrieves a random game from a list of IDs.
    /// </summary>
    /// <param name="gameIds">List of game IDs.</param>
    /// <param name="userId">ID of the owning user.</param>
    /// <returns>A randomly chosen game.</returns>
    Task<BoardGame?> GetRandomGameAsync(IEnumerable<int> gameIds, string userId);
    
    /// <summary>
    /// Retrieves the list of distinct game types for a user.
    /// </summary>
    /// <param name="userId">ID of the owning user.</param>
    /// <returns>List of game types.</returns>
    Task<IEnumerable<string>> GetDistinctGameTypesAsync(string userId);
}
