using GameOnTonight.Domain.Entities;
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
            
        builder.HasOne(s => s.BoardGame)
            .WithMany(g => g.GameSessions)
            .HasForeignKey(s => s.BoardGameId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.BoardGameId);
        builder.HasIndex(s => s.PlayedAt);
    }
}
