using GameOnTonight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameOnTonight.Infrastructure.Configurations;

/// <summary>
/// EF Core configuration for the GameSessionPlayer entity.
/// </summary>
public class GameSessionPlayerConfiguration : BaseConfiguration<GameSessionPlayer>
{
    public override void Configure(EntityTypeBuilder<GameSessionPlayer> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("GameSessionPlayers");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id)
            .UseIdentityColumn()
            .IsRequired();

        builder.Property(p => p.GameSessionId)
            .IsRequired();

        builder.Property(p => p.PlayerName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(p => p.Score)
            .IsRequired(false);

        builder.Property(p => p.IsWinner)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(p => p.Position)
            .IsRequired(false);

        builder.Property(p => p.GroupMemberId)
            .IsRequired(false);

        // Configure relationship to GroupMember
        builder.HasOne(p => p.GroupMember)
            .WithMany()
            .HasForeignKey(p => p.GroupMemberId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(p => p.GameSessionId);
    }
}
