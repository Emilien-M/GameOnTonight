using GameOnTonight.Domain.Entities.Common;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

/// <summary>
/// Implémentation du pattern Unit of Work pour coordonner les opérations entre repositories
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly IEntityValidationService _validationService;
    private bool _disposed = false;

    public UnitOfWork(ApplicationDbContext context, IEntityValidationService validationService)
    {
        _context = context;
        _validationService = validationService;
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
        
        // Déléguer la validation au service
        _validationService.ValidateEntities(changedEntities);
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
