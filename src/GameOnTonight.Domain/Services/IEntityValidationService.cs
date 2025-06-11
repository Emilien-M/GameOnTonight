using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Services;

/// <summary>
/// Interface du service de validation des entités du domaine
/// </summary>
public interface IEntityValidationService
{
    /// <summary>
    /// Valide une collection d'entités et lève une exception si des erreurs de domaine sont détectées
    /// </summary>
    /// <param name="entities">Collection d'entités à valider</param>
    void ValidateEntities(IEnumerable<BaseEntity> entities);
}
