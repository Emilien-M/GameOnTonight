using GameOnTonight.Domain.Entities.Common;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

/// <summary>
/// Implémentation du pattern Unit of Work pour coordonner les opérations entre repositories
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private bool _disposed = false;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        // Vérifier toutes les entités modifiées ou ajoutées pour s'assurer qu'elles sont valides
        ValidateEntities();
        
        return await _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// Vérifie que toutes les entités modifiées ou ajoutées ne contiennent pas d'erreurs de domaine
    /// </summary>
    private void ValidateEntities()
    {
        // Récupérer toutes les entités ajoutées ou modifiées qui héritent de BaseEntity
        var changedEntities = _context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified 
                   && e.Entity is BaseEntity)
            .Select(e => (e.Entity as BaseEntity)!)
            .ToList();
        
        // S'il n'y a pas d'entités modifiées, pas besoin de vérification
        if (changedEntities.Count == 0)
        {
            return;
        }
        
        // Vérifier s'il existe des entités avec des erreurs de domaine
        var entitiesWithErrors = changedEntities
            .Where(entity => entity.HasErrors)
            .ToList();
            
        if (entitiesWithErrors.Count > 0)
        {
            // Collecter toutes les erreurs de domaine
            var allErrors = entitiesWithErrors
                .SelectMany(entity => entity.DomainErrors)
                .ToList();
                
            // Lever une DomainException avec toutes les erreurs collectées
            throw new DomainException(allErrors);
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }
}
