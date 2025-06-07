using System.Linq.Expressions;
using GameOnTonight.Domain.Entities.Common;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

/// <summary>
/// Implémentation générique du Repository Pattern avec Entity Framework Core
/// </summary>
/// <typeparam name="TEntity">Type de l'entité</typeparam>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<TEntity> DbSet;
    private readonly ICurrentUserService _currentUserService;

    public Repository(ApplicationDbContext context, ICurrentUserService currentUserService)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
        _currentUserService = currentUserService;
    }

    public async Task<TEntity?> GetByIdAsync(object id, string userId)
    {
        var entity = await DbSet.FindAsync(id);
        
        // Si l'entité est null ou n'implémente pas IUserOwnedEntity, retourner null
        if (entity == null || !(entity is IUserOwnedEntity userOwnedEntity))
            return null;
        
        // Vérifier que l'entité appartient à l'utilisateur
        return userOwnedEntity.UserId == userId ? entity : null;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(string userId)
    {
        // Récupérer seulement les entités qui appartiennent à l'utilisateur
        if (typeof(IUserOwnedEntity).IsAssignableFrom(typeof(TEntity)))
        {
            return await DbSet.OfType<IUserOwnedEntity>()
                .Where(e => e.UserId == userId)
                .Cast<TEntity>()
                .ToListAsync();
        }
        
        // Si l'entité n'implémente pas IUserOwnedEntity, retourner une liste vide
        return Enumerable.Empty<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, string? userId = null)
    {
        // Si userId est fourni et que l'entité implémente IUserOwnedEntity
        if (!string.IsNullOrEmpty(userId) && typeof(IUserOwnedEntity).IsAssignableFrom(typeof(TEntity)))
        {
            // Créer un prédicat composite pour filtrer par userId également
            var parameter = Expression.Parameter(typeof(TEntity));
            var userIdProperty = Expression.Property(parameter, "UserId");
            var userIdConstant = Expression.Constant(userId);
            var userIdEquals = Expression.Equal(userIdProperty, userIdConstant);
            var userIdPredicate = Expression.Lambda<Func<TEntity, bool>>(userIdEquals, parameter);
            
            // Combiner les prédicats
            var combinedPredicate = PredicateBuilder.And(predicate, userIdPredicate);
            
            return await DbSet.Where(combinedPredicate).ToListAsync();
        }
        
        // Si userId n'est pas fourni, on utilise le prédicat tel quel
        return await DbSet.Where(predicate).ToListAsync();
    }

    public async Task AddAsync(TEntity entity)
    {
        // Vérifier que l'utilisateur est authentifié
        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedAccessException("L'utilisateur doit être authentifié pour ajouter une entité.");
            
        string userId = _currentUserService.UserId!;
            
        // Si l'entité implémente IUserOwnedEntity, définir l'userId
        if (entity is IUserOwnedEntity userOwnedEntity)
        {
            userOwnedEntity.SetUserId(userId);
        }
        
        await DbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities)
    {
        // Vérifier que l'utilisateur est authentifié
        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedAccessException("L'utilisateur doit être authentifié pour ajouter des entités.");
            
        string userId = _currentUserService.UserId!;
            
        // Définir l'userId pour chaque entité qui implémente IUserOwnedEntity
        foreach (var entity in entities)
        {
            if (entity is IUserOwnedEntity userOwnedEntity)
            {
                userOwnedEntity.SetUserId(userId);
            }
        }
        
        await DbSet.AddRangeAsync(entities);
    }

    public async Task<bool> RemoveAsync(TEntity entity)
    {
        // Vérifier que l'utilisateur est authentifié
        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedAccessException("L'utilisateur doit être authentifié pour supprimer une entité.");
            
        string userId = _currentUserService.UserId!;
            
        // Si l'entité n'implémente pas IUserOwnedEntity ou si elle n'appartient pas à l'utilisateur courant
        if (entity is IUserOwnedEntity userOwnedEntity && userOwnedEntity.UserId != userId)
            return false;
        
        DbSet.Remove(entity);
        return true;
    }

    public async Task<int> RemoveRangeAsync(IEnumerable<TEntity> entities)
    {
        // Vérifier que l'utilisateur est authentifié
        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedAccessException("L'utilisateur doit être authentifié pour supprimer des entités.");
            
        string userId = _currentUserService.UserId!;
            
        var entitiesToRemove = entities
            .OfType<IUserOwnedEntity>()
            .Where(e => e.UserId == userId)
            .Cast<TEntity>()
            .ToList();
        
        if (entitiesToRemove.Count == 0)
            return 0;
        
        DbSet.RemoveRange(entitiesToRemove);
        return entitiesToRemove.Count;
    }

    public async Task<bool> UpdateAsync(TEntity entity)
    {
        // Vérifier que l'utilisateur est authentifié
        if (!_currentUserService.IsAuthenticated)
            throw new UnauthorizedAccessException("L'utilisateur doit être authentifié pour mettre à jour une entité.");
            
        string userId = _currentUserService.UserId!;
            
        // Si l'entité n'implémente pas IUserOwnedEntity ou si elle n'appartient pas à l'utilisateur courant
        if (entity is IUserOwnedEntity userOwnedEntity && userOwnedEntity.UserId != userId)
            return false;
        
        DbSet.Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;
        return true;
    }
}

/// <summary>
/// Classe d'assistance pour combiner des expressions de prédicats
/// </summary>
public static class PredicateBuilder
{
    /// <summary>
    /// Combine deux prédicats avec une opération AND
    /// </summary>
    public static Expression<Func<T, bool>> And<T>(Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
    {
        var parameter = Expression.Parameter(typeof(T));

        var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
        var left = leftVisitor.Visit(expr1.Body);

        var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
        var right = rightVisitor.Visit(expr2.Body);

        return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
    }

    private class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly Expression _oldValue;
        private readonly Expression _newValue;

        public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
        {
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override Expression Visit(Expression node)
        {
            return node == _oldValue ? _newValue : base.Visit(node);
        }
    }
}
