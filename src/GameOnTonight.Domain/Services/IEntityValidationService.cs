using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Services;

/// <summary>
/// Interface for the domain entity validation service.
/// </summary>
public interface IEntityValidationService
{
    /// <summary>
    /// Validates a collection of entities and throws an exception if domain errors are detected.
    /// </summary>
    /// <param name="entities">Collection of entities to validate.</param>
    void ValidateEntities(params BaseEntity[] entities);
}
