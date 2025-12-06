using GameOnTonight.Domain.Entities.Common;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Services;

namespace GameOnTonight.Infrastructure.Services;

/// <summary>
/// Implementation of the domain entity validation service.
/// </summary>
public class EntityValidationService : IEntityValidationService
{
    /// <summary>
    /// Validates a collection of entities and throws an exception if domain errors are detected.
    /// </summary>
    /// <param name="entities">Collection of entities to validate.</param>
    public void ValidateEntities(params BaseEntity[] entities)
    {
        var entitiesList = entities.ToList();
        if (entitiesList.Count == 0)
        {
            return;
        }
        
        var entitiesWithErrors = entitiesList
            .Where(entity => entity.HasErrors)
            .ToList();
            
        if (entitiesWithErrors.Count > 0)
        {
            var allErrors = entitiesWithErrors
                .SelectMany(entity => entity.DomainErrors)
                .ToList();
                
            throw new DomainException(allErrors);
        }
    }
}
