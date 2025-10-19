using GameOnTonight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameOnTonight.Infrastructure.Configurations;

public class ProfilConfiguration : BaseConfiguration<Profil>
{
    public override void Configure(EntityTypeBuilder<Profil> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Profils");
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasOne(x => x.User).WithOne();
    }
}