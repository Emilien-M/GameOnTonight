using GameOnTonight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameOnTonight.Infrastructure.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité GameSession
/// </summary>
public class GameSessionConfiguration : BaseConfiguration<GameSession>
{
    public void Configure(EntityTypeBuilder<GameSession> builder)
    {
        // Configuration de la table
        builder.ToTable("GameSessions");
        
        // Clé primaire
        builder.HasKey(s => s.Id);
        
        // Propriétés de base
        builder.Property(s => s.Id)
            .UseIdentityColumn()
            .IsRequired();
            
        builder.Property(s => s.CreatedAt)
            .IsRequired();
            
        builder.Property(s => s.UpdatedAt)
            .IsRequired(false);
            
        // Propriété du UserOwnedEntity
        builder.Property(s => s.UserId)
            .IsRequired()
            .HasMaxLength(450); // Correspond à la taille de l'ID dans ASP.NET Identity
        
        // Propriétés spécifiques à la session
        builder.Property(s => s.BoardGameId)
            .IsRequired();
            
        builder.Property(s => s.PlayedAt)
            .IsRequired();
            
        builder.Property(s => s.PlayerCount)
            .IsRequired();
            
        builder.Property(s => s.Notes)
            .IsRequired(false)
            .HasMaxLength(1000);
            
        // Relation avec le jeu de société
        builder.HasOne(s => s.BoardGame)
            .WithMany(g => g.GameSessions)
            .HasForeignKey(s => s.BoardGameId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Index
        builder.HasIndex(s => s.UserId);
        builder.HasIndex(s => s.BoardGameId);
        builder.HasIndex(s => s.PlayedAt);
    }
}
