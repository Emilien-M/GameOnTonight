using GameOnTonight.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameOnTonight.Infrastructure.Configurations;

/// <summary>
/// EF Core configuration for the GameType entity.
/// </summary>
public class GameTypeConfiguration : BaseConfiguration<GameType>
{
    public override void Configure(EntityTypeBuilder<GameType> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("GameTypes");
        
        builder.HasKey(gt => gt.Id);
        
        builder.Property(gt => gt.Id)
            .UseIdentityColumn()
            .IsRequired();
            
        builder.Property(gt => gt.CreatedAt)
            .IsRequired();
            
        builder.Property(gt => gt.UpdatedAt)
            .IsRequired(false);
            
        builder.Property(gt => gt.UserId)
            .IsRequired()
            .HasMaxLength(450);
        
        builder.Property(gt => gt.Name)
            .IsRequired()
            .HasMaxLength(100);
        
        // Configure foreign key relationship to IdentityUser without navigation property
        builder.HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(gt => gt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Many-to-many relationship with BoardGame is configured in BoardGameConfiguration
        
        builder.HasIndex(gt => gt.UserId);
        builder.HasIndex(gt => new { gt.UserId, gt.Name }).IsUnique();
    }
}
