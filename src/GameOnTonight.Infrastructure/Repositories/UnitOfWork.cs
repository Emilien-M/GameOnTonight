using GameOnTonight.Domain.Entities.Common;
using GameOnTonight.Domain.Exceptions;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

/// <summary>
/// Implementation of the Unit of Work pattern to coordinate operations between repositories.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private IReadOnlyList<ITransactionalElement> _transactionalElements;
    private bool _disposed;

    public bool IsInTransaction { get; private set;  }

    public UnitOfWork(IEnumerable<ITransactionalElement> transactionalElements)
    {
        _transactionalElements = transactionalElements.ToList();
    }

    public async Task<ITransactionalElement> BeginTransactionAsync()
    {
        foreach (var transactionalElement in _transactionalElements)
            await transactionalElement.BeginTransactionAsync();
        
        IsInTransaction = true;
        return this;
    }

    public async Task CommitAsync()
    {
        if (!IsInTransaction)
            return;
        
        foreach (var transactionalElement in _transactionalElements)
            await transactionalElement.CommitAsync();
        
        IsInTransaction = false;
    }

    public async Task RollbackAsync()
    {
        if (!IsInTransaction)
            return;
        
        foreach (var transactionalElement in _transactionalElements)
            await transactionalElement.RollbackAsync();
        
        IsInTransaction = false;
    }
    

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            foreach (var transactionalElement in _transactionalElements)
            {
                transactionalElement.Dispose();
            }
        }
        _disposed = true;
    }
}
