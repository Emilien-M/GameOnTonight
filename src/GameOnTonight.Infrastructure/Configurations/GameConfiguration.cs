using GameOnTonight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameOnTonight.Infrastructure.Configurations;

/// <summary>
/// Configuration EF Core pour l'entité Game
/// </summary>
public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        // Configuration de la table
        builder.ToTable("Games");
        
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
            .HasMaxLength(450); // Corresponde à la taille de l'ID dans ASP.NET Identity
        
        // Propriétés spécifiques au jeu
        builder.Property(g => g.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(g => g.Description)
            .IsRequired(false)
            .HasMaxLength(2000);
            
        builder.Property(g => g.IsActive)
            .IsRequired()
            .HasDefaultValue(true);
            
        // Index
        builder.HasIndex(g => g.UserId);
        builder.HasIndex(g => g.Name);
        builder.HasIndex(g => g.IsActive);
    }
}
