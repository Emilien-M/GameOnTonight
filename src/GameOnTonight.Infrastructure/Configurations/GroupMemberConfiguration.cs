using GameOnTonight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameOnTonight.Infrastructure.Configurations;

public class GroupMemberConfiguration : BaseConfiguration<GroupMember>
{
    public override void Configure(EntityTypeBuilder<GroupMember> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("GroupMembers");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(m => m.Role)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.JoinedAt)
            .IsRequired();

        builder.Property(m => m.CreatedAt)
            .IsRequired();

        builder.Property(m => m.UpdatedAt);

        // Indexes
        builder.HasIndex(m => new { m.GroupId, m.UserId })
            .IsUnique();

        builder.HasIndex(m => m.UserId);

        // Relationships
        builder.HasOne(m => m.Profile)
            .WithMany()
            .HasForeignKey(m => m.UserId)
            .HasPrincipalKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
