namespace GameOnTonight.Domain.Entities.Common;

public interface ITransactionalElement : IDisposable
{
    bool IsInTransaction { get; }
    
    Task<ITransactionalElement> BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}