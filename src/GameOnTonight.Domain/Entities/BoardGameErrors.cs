using GameOnTonight.Domain.Exceptions;

namespace GameOnTonight.Domain.Entities;

public static class BoardGameErrors
{
    public static Func<int, DomainError> BoardGameNotFound => id => 
        new DomainError( $"Board game with ID {id} not found.", nameof(BoardGameNotFound));
}