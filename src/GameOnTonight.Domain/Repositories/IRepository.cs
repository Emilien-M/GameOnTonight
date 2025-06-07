using System.Linq.Expressions;
using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Domain.Repositories;

/// <summary>
/// Interface générique pour le repository pattern
/// </summary>
/// <typeparam name="TEntity">Type de l'entité</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Récupère une entité par son ID et l'ID de son propriétaire
    /// </summary>
    /// <param name="id">ID de l'entité</param>
    /// <param name="userId">ID de l'utilisateur propriétaire</param>
    /// <returns>L'entité trouvée ou null</returns>
    Task<TEntity?> GetByIdAsync(object id, string userId);
    
    /// <summary>
    /// Récupère toutes les entités appartenant à un utilisateur spécifique
    /// </summary>
    /// <param name="userId">ID de l'utilisateur propriétaire</param>
    /// <returns>Liste des entités</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(string userId);
    
    /// <summary>
    /// Trouve des entités selon un prédicat
    /// </summary>
    /// <param name="predicate">Expression de filtrage</param>
    /// <param name="userId">ID de l'utilisateur propriétaire (optionnel pour les requêtes spéciales)</param>
    /// <returns>Liste des entités filtrées</returns>
    Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, string? userId = null);
    
    /// <summary>
    /// Ajoute une nouvelle entité
    /// </summary>
    /// <param name="entity">Entité à ajouter</param>
    Task AddAsync(TEntity entity);
    
    /// <summary>
    /// Ajoute plusieurs entités
    /// </summary>
    /// <param name="entities">Entités à ajouter</param>
    Task AddRangeAsync(IEnumerable<TEntity> entities);
    
    /// <summary>
    /// Supprime une entité
    /// </summary>
    /// <param name="entity">Entité à supprimer</param>
    /// <returns>True si l'entité a été supprimée, False si elle n'appartient pas à l'utilisateur courant</returns>
    Task<bool> RemoveAsync(TEntity entity);
    
    /// <summary>
    /// Supprime plusieurs entités
    /// </summary>
    /// <param name="entities">Entités à supprimer</param>
    /// <returns>Nombre d'entités supprimées</returns>
    Task<int> RemoveRangeAsync(IEnumerable<TEntity> entities);
    
    /// <summary>
    /// Met à jour une entité
    /// </summary>
    /// <param name="entity">Entité à mettre à jour</param>
    /// <returns>True si l'entité a été mise à jour, False si elle n'appartient pas à l'utilisateur courant</returns>
    Task<bool> UpdateAsync(TEntity entity);
}
