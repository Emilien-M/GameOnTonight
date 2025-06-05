namespace GameOnTonight.Domain.Exceptions;

/// <summary>
/// Exception thrown when business rules are violated in the domain.
/// </summary>
public class DomainException : Exception
{
    public IReadOnlyList<DomainError> Errors { get; }
    
    public DomainException(string message, string propertyName) 
        : base(message)
    {
        Errors = new List<DomainError> { new DomainError(message, propertyName) };
    }

    public DomainException(IEnumerable<(string propertyName, string message)> errors)
    {
        Errors = errors.Select(e => new DomainError(e.message, e.propertyName)).ToList().AsReadOnly();
    }

    public DomainException(DomainError error) 
        : this(new List<DomainError> { error })
    {
    }
    
    public DomainException(IEnumerable<DomainError> errors) 
        : base("One or more domain validation errors have occurred.")
    {
        Errors = errors.ToList().AsReadOnly();
    }
}
