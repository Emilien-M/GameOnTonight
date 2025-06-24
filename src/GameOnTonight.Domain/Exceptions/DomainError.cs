namespace GameOnTonight.Domain.Exceptions;

/// <summary>
/// Represents a business validation error in the domain.
/// </summary>
public class DomainError
{
    /// <summary>
    /// Error message describing the business rule violation.
    /// </summary>
    public string Message { get; }
    
    /// <summary>
    /// Property affected by the error (can be null if the error concerns the entire entity).
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    /// Creates a new domain error.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="propertyName">Affected property (optional).</param>
    public DomainError(string message, string propertyName = null)
    {
        Message = message;
        PropertyName = propertyName ?? string.Empty;
    }
}
