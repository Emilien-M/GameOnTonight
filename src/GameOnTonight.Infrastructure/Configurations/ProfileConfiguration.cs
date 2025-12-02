using GameOnTonight.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameOnTonight.Infrastructure.Configurations;

public class ProfileConfiguration : BaseConfiguration<Profile>
{
    public override void Configure(EntityTypeBuilder<Profile> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Profiles");
        builder.HasIndex(x => x.UserId).IsUnique();
        
        // Configure foreign key relationship without navigation property in domain
        builder.HasOne<IdentityUser>()
            .WithOne()
            .HasForeignKey<Profile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
