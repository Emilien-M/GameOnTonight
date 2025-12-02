using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Domain.Repositories;

public interface IProfileRepository : IRepository<Profile>
{
    Task<Profile?> GetAsync(CancellationToken cancellationToken);
}
