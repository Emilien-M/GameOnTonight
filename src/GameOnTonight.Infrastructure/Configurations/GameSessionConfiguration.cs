using GameOnTonight.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameOnTonight.Infrastructure.Configurations;

/// <summary>
/// EF Core configuration for the GameSession entity.
/// </summary>
public class GameSessionConfiguration : BaseConfiguration<GameSession>
{
    public override void Configure(EntityTypeBuilder<GameSession> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("GameSessions");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Id)
            .UseIdentityColumn()
            .IsRequired();
            
        builder.Property(s => s.CreatedAt)
            .IsRequired();
            
        builder.Property(s => s.UpdatedAt)
            .IsRequired(false);
            
        builder.Property(s => s.UserId)
            .IsRequired()
            .HasMaxLength(450); 
        
        builder.Property(s => s.BoardGameId)
            .IsRequired();
            
        builder.Property(s => s.PlayedAt)
            .IsRequired();
            
        builder.Property(s => s.PlayerCount)
            .IsRequired();
            
        builder.Property(s => s.Notes)
            .IsRequired(false)
            .HasMaxLength(1000);

        builder.Property(s => s.Rating)
            .IsRequired(false);

        builder.Property(s => s.PhotoUrl)
            .IsRequired(false)
            .HasMaxLength(500);
            
        builder.HasOne(s => s.BoardGame)
            .WithMany(g => g.GameSessions)
            .HasForeignKey(s => s.BoardGameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Players)
            .WithOne(p => p.GameSession)
            .HasForeignKey(p => p.GameSessionId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configure foreign key relationship to IdentityUser without navigation property
        builder.HasOne<IdentityUser>()
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Configure relationship to Group for sharing
        builder.HasOne(s => s.Group)
            .WithMany()
            .HasForeignKey(s => s.GroupId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.BoardGameId);
        builder.HasIndex(s => s.PlayedAt);
        builder.HasIndex(s => s.GroupId)
            .HasFilter("\"GroupId\" IS NOT NULL");
    }
}
