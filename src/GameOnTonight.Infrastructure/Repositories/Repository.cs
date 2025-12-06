using System.Linq.Expressions;
using GameOnTonight.Domain.Entities.Common;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

/// <summary>
/// Generic implementation of the Repository Pattern with Entity Framework Core.
/// </summary>
/// <typeparam name="TEntity">Type of the entity.</typeparam>
public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly ApplicationDbContext Context;
    protected readonly DbSet<TEntity> DbSet;
    protected readonly ICurrentUserService CurrentUserService;
    private readonly IEntityValidationService _entityValidationService;

    public Repository(ApplicationDbContext context, ICurrentUserService currentUserService, IEntityValidationService entityValidationService)
    {
        Context = context;
        DbSet = context.Set<TEntity>();
        CurrentUserService = currentUserService;
        _entityValidationService = entityValidationService;
    }

    public async Task<TEntity?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
    {
        var entity = await DbSet.FindAsync([id], cancellationToken);
        
        if (entity == null || !(entity is IUserOwnedEntity userOwnedEntity))
            return null;
        
        return userOwnedEntity.UserId == CurrentUserService.UserId ? entity : null;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        if (typeof(IUserOwnedEntity).IsAssignableFrom(typeof(TEntity)))
        {
            return await DbSet.OfType<IUserOwnedEntity>()
                .Where(e => e.UserId == CurrentUserService.UserId)
                .Cast<TEntity>()
                .ToListAsync(cancellationToken);
        }
        
        return Enumerable.Empty<TEntity>();
    }

    public async Task<(IEnumerable<TEntity> Items, int TotalCount)> GetAllPaginatedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        if (typeof(IUserOwnedEntity).IsAssignableFrom(typeof(TEntity)))
        {
            var query = DbSet.OfType<IUserOwnedEntity>()
                .Where(e => e.UserId == CurrentUserService.UserId);
            
            var totalCount = await query.CountAsync(cancellationToken);
            
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Cast<TEntity>()
                .ToListAsync(cancellationToken);
            
            return (items, totalCount);
        }
        
        return (Enumerable.Empty<TEntity>(), 0);
    }

    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        var userId = CurrentUserService.UserId;
        
        if (!string.IsNullOrEmpty(userId) && typeof(IUserOwnedEntity).IsAssignableFrom(typeof(TEntity)))
        {
            var parameter = Expression.Parameter(typeof(TEntity));
            var userIdProperty = Expression.Property(parameter, "UserId");
            var userIdConstant = Expression.Constant(userId);
            var userIdEquals = Expression.Equal(userIdProperty, userIdConstant);
            var userIdPredicate = Expression.Lambda<Func<TEntity, bool>>(userIdEquals, parameter);
            
            var combinedPredicate = PredicateBuilder.And(predicate, userIdPredicate);
            
            return await DbSet.Where(combinedPredicate).ToListAsync(cancellationToken);
        }
        
        return await DbSet.Where(predicate).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        EnsureUserAuthenticated();
        
        _entityValidationService.ValidateEntities(entity);
            
        var userId = CurrentUserService.UserId!;
            
        if (entity is IUserOwnedEntity userOwnedEntity)
        {
            userOwnedEntity.SetUserId(userId);
        }
        
        await DbSet.AddAsync(entity, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();
        
        EnsureUserAuthenticated();
        
        _entityValidationService.ValidateEntities(entityList.ToArray<BaseEntity>());

        var userId = CurrentUserService.UserId!;
            
        foreach (var entity in entityList)
        {
            if (entity is IUserOwnedEntity userOwnedEntity)
            {
                userOwnedEntity.SetUserId(userId);
            }
        }
        
        await DbSet.AddRangeAsync(entityList, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RemoveAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        EnsureUserAuthenticatedAndHasPermissionAsync(entity);
        
        _entityValidationService.ValidateEntities(entity);
        
        DbSet.Remove(entity);
        await Context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<int> RemoveRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var entityList = entities.ToList();
        
        EnsureUserAuthenticatedAndHasPermissionAsync(entityList);
        
        _entityValidationService.ValidateEntities(entityList.ToArray<BaseEntity>());
        
        DbSet.RemoveRange(entityList);
        await Context.SaveChangesAsync(cancellationToken);
        return entityList.Count;
    }

    public async Task<bool> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        EnsureUserAuthenticatedAndHasPermissionAsync(entity);
        
        _entityValidationService.ValidateEntities(entity);
        
        DbSet.Attach(entity);
        Context.Entry(entity).State = EntityState.Modified;
        await Context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private void EnsureUserAuthenticated()
    {
        if (!CurrentUserService.IsAuthenticated)
            throw new UnauthorizedAccessException("User must be authenticated to update an entity.");
    }
    
    private void EnsureUserAuthenticatedAndHasPermissionAsync(TEntity entity)
    {
        EnsureUserAuthenticated();
            
        var userId = CurrentUserService.UserId!;
            
        if (entity is IUserOwnedEntity userOwnedEntity && userOwnedEntity.UserId != userId)
            throw new UnauthorizedAccessException("User does not have permission to update this entity.");
    }

    private void EnsureUserAuthenticatedAndHasPermissionAsync(IEnumerable<TEntity> entities)
    {
        EnsureUserAuthenticated();
        
        var userId = CurrentUserService.UserId!;
            
        foreach (var entity in entities)
        {
            if (entity is IUserOwnedEntity userOwnedEntity && userOwnedEntity.UserId != userId)
                throw new UnauthorizedAccessException("User does not have permission to update this entity.");
        }
    }
}

/// <summary>
/// Helper class to combine predicate expressions.
/// </summary>
public static class PredicateBuilder
{
    /// <summary>
    /// Combines two predicates with an AND operation.
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
