using GameOnTonight.Domain.Entities.Common;

namespace GameOnTonight.Domain.Repositories;

public interface IUnitOfWork : ITransactionalElement, IDisposable
{
}
