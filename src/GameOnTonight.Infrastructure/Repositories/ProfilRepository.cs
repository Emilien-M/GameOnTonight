using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

public class ProfilRepository : Repository<Profil>, IProfilRepository
{
    public ProfilRepository(ApplicationDbContext context, ICurrentUserService currentUserService) 
        : base(context, currentUserService)
    {
    }

    public Task<Profil?> GetAsync(CancellationToken cancellationToken)
    {
        return DbSet.SingleOrDefaultAsync(cancellationToken);
    }
}