using GameOnTonight.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameOnTonight.Infrastructure.Configurations;

public class GroupInviteCodeConfiguration : BaseConfiguration<GroupInviteCode>
{
    public override void Configure(EntityTypeBuilder<GroupInviteCode> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("GroupInviteCodes");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(c => c.ExpiresAt)
            .IsRequired();

        builder.Property(c => c.CreatedByUserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(c => c.CreatedAt)
            .IsRequired();

        builder.Property(c => c.UpdatedAt);

        // Indexes
        builder.HasIndex(c => c.Code)
            .IsUnique();

        builder.HasIndex(c => c.ExpiresAt);

        builder.HasIndex(c => c.GroupId);
    }
}
