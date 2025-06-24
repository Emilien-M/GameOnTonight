namespace GameOnTonight.Domain.Exceptions;

/// <summary>
/// Exception thrown when business rules are violated in the domain.
/// </summary>
public class DomainException : Exception
{
    /// <summary>
    /// List of domain errors associated with this exception.
    /// </summary>
    public IReadOnlyList<DomainError> Errors { get; }

    /// <summary>
    /// Creates a new domain exception with a single error.
    /// </summary>
    /// <param name="message">Error message.</param>
    public DomainException(string message) 
        : base(message)
    {
        Errors = new List<DomainError> { new DomainError(message) };
    }

    /// <summary>
    /// Creates a new domain exception with a single error.
    /// </summary>
    /// <param name="message">Error message.</param>
    /// <param name="propertyName">Property affected by the error.</param>
    public DomainException(string message, string propertyName) 
        : base(message)
    {
        Errors = new List<DomainError> { new DomainError(message, propertyName) };
    }

    public DomainException(IEnumerable<(string propertyName, string message)> errors)
    {
        Errors = errors.Select(e => new DomainError(e.message, e.propertyName)).ToList().AsReadOnly();
    }

    /// <summary>
    /// Creates a new domain exception with a list of errors.
    /// </summary>
    /// <param name="errors">List of domain errors.</param>
    public DomainException(IEnumerable<DomainError> errors) 
        : base("One or more domain validation errors have occurred.")
    {
        Errors = errors.ToList().AsReadOnly();
    }
}
