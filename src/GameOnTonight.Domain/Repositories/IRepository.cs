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
    /// <returns>The found entity or null.</returns>
    Task<TEntity?> GetByIdAsync(object id);
    
    /// <summary>
    /// Retrieves all entities belonging to a specific user.
    /// </summary>
    /// <returns>List of entities.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync();
    
    /// <summary>
    /// Finds entities according to a predicate.
    /// </summary>
    /// <param name="predicate">Filtering expression.</param>
    /// <returns>List of filtered entities.</returns>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
    
    /// <summary>
    /// Adds a new entity.
    /// </summary>
    /// <param name="entity">Entity to add.</param>
    Task AddAsync(TEntity entity);
    
    /// <summary>
    /// Adds several entities.
    /// </summary>
    /// <param name="entities">Entities to add.</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    
    /// <summary>
    /// Deletes an entity.
    /// </summary>
    /// <param name="entity">Entity to delete.</param>
    /// <returns>True if the entity was deleted, False if it does not belong to the current user.</returns>
    Task<bool> RemoveAsync(TEntity entity);
    
    /// <summary>
    /// Deletes several entities.
    /// </summary>
    /// <param name="entities">Entities to delete.</param>
    /// <returns>Number of deleted entities.</returns>
    Task<int> RemoveRangeAsync(IEnumerable<TEntity> entities);
    
    /// <summary>
    /// Updates an entity.
    /// </summary>
    /// <param name="entity">Entity to update.</param>
    /// <returns>True if the entity was updated, False if it does not belong to the current user.</returns>
    Task<bool> UpdateAsync(TEntity entity);
}
