using GameOnTonight.Domain.Entities;
using GameOnTonight.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameOnTonight.Infrastructure.Configurations;

/// <summary>
/// EF Core configuration for the BoardGame entity.
/// </summary>
public class BoardGameConfiguration : BaseConfiguration<BoardGame>
{
    private readonly ICurrentUserService _currentUserService;

    public BoardGameConfiguration(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }
    
    public override void Configure(EntityTypeBuilder<BoardGame> builder)
    {
        base.Configure(builder);
        
        builder.HasQueryFilter(x => x.UserId == _currentUserService.UserId);
        
        builder.ToTable("BoardGames");
        
        builder.HasKey(g => g.Id);
        
        builder.Property(g => g.Id)
            .UseIdentityColumn()
            .IsRequired();
            
        builder.Property(g => g.CreatedAt)
            .IsRequired();
            
        builder.Property(g => g.UpdatedAt)
            .IsRequired(false);
            
        builder.Property(g => g.UserId)
            .IsRequired()
            .HasMaxLength(450); 
        
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
            
        builder.HasMany(g => g.GameSessions)
            .WithOne(s => s.BoardGame)
            .HasForeignKey(s => s.BoardGameId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(g => g.UserId);
        builder.HasIndex(g => g.Name);
        builder.HasIndex(g => g.GameType);
        builder.HasIndex(g => new { g.MinPlayers, g.MaxPlayers });
    }
}
