using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Domain.Repositories;

/// <summary>
/// Repository interface for the GameSession entity.
/// </summary>
public interface IGameSessionRepository : IRepository<GameSession>
{
    /// <summary>
    /// Retrieves a user's game history, ordered by descending date.
    /// </summary>
    /// <param name="userId">ID of the owning user.</param>
    /// <param name="count">Maximum number of entries to retrieve (optional).</param>
    /// <returns>List of game sessions.</returns>
    Task<IEnumerable<GameSession>> GetSessionHistoryAsync(string userId, int? count = null);
    
    /// <summary>
    /// Retrieves sessions for a specific game.
    /// </summary>
    /// <param name="boardGameId">ID of the game.</param>
    /// <param name="userId">ID of the owning user.</param>
    /// <returns>List of sessions associated with the game.</returns>
    Task<IEnumerable<GameSession>> GetSessionsByGameAsync(int boardGameId, string userId);
    
    /// <summary>
    /// Counts the number of games played for each of a user's games.
    /// </summary>
    /// <param name="userId">ID of the owning user.</param>
    /// <returns>Dictionary associating the game ID with the number of games.</returns>
    Task<IDictionary<int, int>> GetGamePlayCountsAsync(string userId);
}
