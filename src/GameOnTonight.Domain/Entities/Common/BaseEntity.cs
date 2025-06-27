namespace GameOnTonight.Domain.Entities.Common;

using System.Collections.Generic;
using GameOnTonight.Domain.Exceptions;

/// <summary>
/// Base entity with common properties.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Unique identifier for the entity.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Date of creation of the entity.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date of last update of the entity.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
    
    #region Domain Validation
    
    private readonly List<DomainError> _domainErrors = new();
    
    /// <summary>
    /// Read-only list of domain errors accumulated by the entity.
    /// </summary>
    public IReadOnlyCollection<DomainError> DomainErrors => _domainErrors.AsReadOnly();
    
    /// <summary>
    /// Checks if the entity has validation errors.
    /// </summary>
    public bool HasErrors => _domainErrors.Count > 0;
    
    /// <summary>
    /// Adds a domain validation error for a specific property.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="message">Error message.</param>
    protected void AddDomainError(string propertyName, string message)
    {
        _domainErrors.Add(new DomainError(message, propertyName));
    }

    /// <summary>
    /// Validates a string property to ensure it is not null or empty.
    /// </summary>
    /// <param name="value">Value to check.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="maxLength">Maximum length (optional).</param>
    /// <returns>True if validation succeeds, otherwise false.</returns>
    protected bool ValidateString(string? value, string propertyName, int? maxLength = null)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            AddDomainError(propertyName, $"The {propertyName} field is required.");
            return false;
        }

        if (maxLength.HasValue && value.Length > maxLength.Value)
        {
            AddDomainError(propertyName, $"The {propertyName} field cannot exceed {maxLength.Value} characters.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validates a numeric property to ensure it is within a valid range.
    /// </summary>
    /// <param name="value">Value to check.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="min">Minimum value (optional).</param>
    /// <param name="max">Maximum value (optional).</param>
    /// <returns>True if validation succeeds, otherwise false.</returns>
    protected bool ValidateNumber<T>(T value, string propertyName, T? min = null, T? max = null) where T : struct, IComparable<T>
    {
        if (min.HasValue && value.CompareTo(min.Value) < 0)
        {
            AddDomainError(propertyName, $"The {propertyName} field must be greater than or equal to {min.Value}.");
            return false;
        }

        if (max.HasValue && value.CompareTo(max.Value) > 0)
        {
            AddDomainError(propertyName, $"The {propertyName} field must be less than or equal to {max.Value}.");
            return false;
        }

        return true;
    }
    
    /// <summary>
    /// Throws a DomainException if domain errors have been accumulated.
    /// </summary>
    public void ThrowIfInvalid()
    {
        if (HasErrors)
        {
            throw new DomainException(_domainErrors);
        }
    }
    
    /// <summary>
    /// Clears all accumulated errors.
    /// </summary>
    protected void ClearDomainErrors()
    {
        _domainErrors.Clear();
    }
    
    #endregion
}
