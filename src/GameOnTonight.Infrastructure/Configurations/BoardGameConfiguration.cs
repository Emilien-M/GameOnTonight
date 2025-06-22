using GameOnTonight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameOnTonight.Infrastructure.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité BoardGame
/// </summary>
public class BoardGameConfiguration : BaseConfiguration<BoardGame>
{
    public void Configure(EntityTypeBuilder<BoardGame> builder)
    {
        // Configuration de la table
        builder.ToTable("BoardGames");
        
        // Clé primaire
        builder.HasKey(g => g.Id);
        
        // Propriétés de base
        builder.Property(g => g.Id)
            .UseIdentityColumn()
            .IsRequired();
            
        builder.Property(g => g.CreatedAt)
            .IsRequired();
            
        builder.Property(g => g.UpdatedAt)
            .IsRequired(false);
            
        // Propriété du UserOwnedEntity
        builder.Property(g => g.UserId)
            .IsRequired()
            .HasMaxLength(450); // Correspond à la taille de l'ID dans ASP.NET Identity
        
        // Propriétés spécifiques au jeu
        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(g => g.MinPlayers)
            .IsRequired();
            
        builder.Property(g => g.MaxPlayers)
            .IsRequired();
            
        builder.Property(g => g.DurationMinutes)
            .IsRequired();
            
        builder.Property(g => g.GameType)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(g => g.Description)
            .IsRequired(false)
            .HasMaxLength(2000);
            
        builder.Property(g => g.ImageUrl)
            .IsRequired(false)
            .HasMaxLength(500);
            
        // Relation avec les sessions de jeu
        builder.HasMany(g => g.GameSessions)
            .WithOne(s => s.BoardGame)
            .HasForeignKey(s => s.BoardGameId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Index
        builder.HasIndex(g => g.UserId);
        builder.HasIndex(g => g.Name);
        builder.HasIndex(g => g.GameType);
        builder.HasIndex(g => new { g.MinPlayers, g.MaxPlayers });
    }
}
