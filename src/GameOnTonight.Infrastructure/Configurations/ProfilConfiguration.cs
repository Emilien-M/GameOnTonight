using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameOnTonight.Infrastructure.Configurations;

public class ProfilConfiguration : BaseConfiguration<Profil>
{
    private readonly ICurrentUserService _currentUserService;

    public ProfilConfiguration(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    public override void Configure(EntityTypeBuilder<Profil> builder)
    {
        base.Configure(builder);
        
        builder.HasQueryFilter(x => x.UserId == _currentUserService.UserId);
        
        builder.ToTable("Profils");
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasOne(x => x.User).WithOne();
    }
}