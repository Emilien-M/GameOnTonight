using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace GameOnTonight.Infrastructure.Interceptors;

/// <summary>
/// EF Core interceptor to automatically update CreatedAt and UpdatedAt properties.
/// </summary>
public class AuditableEntityInterceptor(TimeProvider timeProvider) : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, 
        InterceptionResult<int> result, 
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        
        UpdateAuditableEntities(eventData.Context);
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
    
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData, 
        InterceptionResult<int> result)
    {
        if (eventData.Context is null)
            return base.SavingChanges(eventData, result);
        
        UpdateAuditableEntities(eventData.Context);
        
        return base.SavingChanges(eventData, result);
    }
    
    private void UpdateAuditableEntities(DbContext context)
    {
        var now = timeProvider.GetUtcNow().UtcDateTime;
        
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                // Pour les nouvelles entités, définir CreatedAt
                entry.Property(e => e.CreatedAt).CurrentValue = now;
            }
            else if (entry.State == EntityState.Modified)
            {
                // Pour les entités modifiées, mettre à jour UpdatedAt
                // mais ne pas modifier CreatedAt
                entry.Property(e => e.UpdatedAt).CurrentValue = now;
                
                // Empêcher la modification de CreatedAt
                entry.Property(e => e.CreatedAt).IsModified = false;
            }
        }
    }
}
