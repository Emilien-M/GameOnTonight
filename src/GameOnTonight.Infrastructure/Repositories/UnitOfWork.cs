using GameOnTonight.Domain.Entities.Common;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

/// <summary>
/// Implementation of the Unit of Work pattern to coordinate operations between repositories.
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
        ValidateEntities();
        
        return await _context.SaveChangesAsync();
    }
    
    /// <summary>
    /// Ensures that all modified or added entities do not contain domain errors.
    /// </summary>
    private void ValidateEntities()
    {
        var changedEntities = _context.ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified 
                   && e.Entity is BaseEntity)
            .Select(e => (e.Entity as BaseEntity)!)
            .ToList();
        
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
