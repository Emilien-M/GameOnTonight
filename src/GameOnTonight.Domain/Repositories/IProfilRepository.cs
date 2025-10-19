using GameOnTonight.Domain.Entities;

namespace GameOnTonight.Domain.Repositories;

public interface IProfilRepository : IRepository<Profil>
{
    Task<Profil?> GetAsync(CancellationToken cancellationToken);
}