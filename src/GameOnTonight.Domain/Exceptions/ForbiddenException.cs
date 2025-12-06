namespace GameOnTonight.Domain.Exceptions;

/// <summary>
/// Exception thrown when the user doesn't have permission to perform an action.
/// </summary>
public class ForbiddenException : Exception
{
    public ForbiddenException(string message) : base(message)
    {
    }
}
