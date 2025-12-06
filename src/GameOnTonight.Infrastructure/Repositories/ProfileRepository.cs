using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Repositories;
using GameOnTonight.Domain.Services;
using GameOnTonight.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GameOnTonight.Infrastructure.Repositories;

public class ProfileRepository : Repository<Profile>, IProfileRepository
{
    public ProfileRepository(ApplicationDbContext context, ICurrentUserService currentUserService, IEntityValidationService entityValidationService) 
        : base(context, currentUserService, entityValidationService)
    {
    }

    public Task<Profile?> GetAsync(CancellationToken cancellationToken)
    {
        return DbSet.SingleOrDefaultAsync(p => p.UserId == CurrentUserService.UserId, cancellationToken);
    }
}
