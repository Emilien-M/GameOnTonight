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
    /// Name of the error, typically used for identification or categorization.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Creates a new domain error.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="name">Error name</param>
    public DomainError(string message, string name)
    {
        Message = message;
        Name = name;
    }

    public DomainException Exception()
        => new DomainException(this);
}
