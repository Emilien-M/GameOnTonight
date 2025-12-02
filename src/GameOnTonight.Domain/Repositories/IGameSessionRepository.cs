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
    /// <param name="count">Maximum number of entries to retrieve (optional).</param>
    /// <returns>List of game sessions.</returns>
    Task<IEnumerable<GameSession>> GetSessionHistoryAsync(int? count = null);
    
    /// <summary>
    /// Retrieves sessions for a specific game.
    /// </summary>
    /// <param name="boardGameId">ID of the game.</param>
    /// <returns>List of sessions associated with the game.</returns>
    Task<IEnumerable<GameSession>> GetSessionsByGameAsync(int boardGameId);
    
    /// <summary>
    /// Counts the number of games played for each of a user's games.
    /// </summary>
    /// <returns>Dictionary associating the game ID with the number of games.</returns>
    Task<IDictionary<int, int>> GetGamePlayCountsAsync();
    
    /// <summary>
    /// Retrieves a game session by ID with its associated players.
    /// </summary>
    /// <param name="id">ID of the session.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The session with players, or null if not found.</returns>
    Task<GameSession?> GetByIdWithPlayersAsync(int id, CancellationToken cancellationToken = default);
}
