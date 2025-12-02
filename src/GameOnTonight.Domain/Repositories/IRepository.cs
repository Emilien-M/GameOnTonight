using System.Linq.Expressions;
using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Domain.Repositories;

/// <summary>
/// Generic interface for the repository pattern.
/// </summary>
/// <typeparam name="TEntity">Type of the entity.</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Retrieves an entity by its ID and its owner's ID.
    /// </summary>
    /// <param name="id">ID of the entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The found entity or null.</returns>
    Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves all entities belonging to a specific user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of entities.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Retrieves a paginated list of entities belonging to a specific user.
    /// </summary>
    /// <param name="page">The page number (1-based).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Tuple containing the items and the total count.</returns>
    Task<(IEnumerable<TEntity> Items, int TotalCount)> GetAllPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Finds entities according to a predicate.
    /// </summary>
    /// <param name="predicate">Filtering expression.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of filtered entities.</returns>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds a new entity.
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Adds several entities.
    /// </summary>
    /// <param name="entities">Entities to add.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">Entity to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the entity was deleted, False if it does not belong to the current user.</returns>
    Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deletes several entities.
    /// </summary>
    /// <param name="entities">Entities to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Number of deleted entities.</returns>
    Task<int> RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="entity">Entity to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the entity was updated, False if it does not belong to the current user.</returns>
    Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default);
}
