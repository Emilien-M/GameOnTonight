using GameOnTonight.Domain.Entities.Common;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Services;

namespace GameOnTonight.Infrastructure.Services;

/// <summary>
/// Implémentation du service de validation des entités du domaine
/// </summary>
public class EntityValidationService : IEntityValidationService
{
    /// <summary>
    /// Valide une collection d'entités et lève une exception si des erreurs de domaine sont détectées
    /// </summary>
    /// <param name="entities">Collection d'entités à valider</param>
    public void ValidateEntities(IEnumerable<BaseEntity> entities)
    {
        // S'il n'y a pas d'entités, pas besoin de vérification
        var entitiesList = entities.ToList();
        if (entitiesList.Count == 0)
        {
            return;
        }
        
        // Vérifier s'il existe des entités avec des erreurs de domaine
        var entitiesWithErrors = entitiesList
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
}
