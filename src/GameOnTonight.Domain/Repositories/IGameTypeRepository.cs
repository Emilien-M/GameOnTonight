using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Domain.Repositories;

/// <summary>
/// Repository interface for the GameType entity.
/// </summary>
public interface IGameTypeRepository : IRepository<GameType>
{
    /// <summary>
    /// Retrieves a game type by its name for the current user.
    /// </summary>
    /// <param name="name">Name of the game type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The found game type or null.</returns>
    Task<GameType?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves game types by their names for the current user.
    /// Creates missing types if they don't exist.
    /// </summary>
    /// <param name="names">Names of the game types.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of game types (existing or newly created).</returns>
    Task<IReadOnlyList<GameType>> GetOrCreateByNamesAsync(IEnumerable<string> names, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a game type with the specified name exists for the current user.
    /// </summary>
    /// <param name="name">Name of the game type.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if exists, false otherwise.</returns>
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken = default);
}
