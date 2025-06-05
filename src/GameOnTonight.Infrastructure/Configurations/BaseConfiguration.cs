using GameOnTonight.Domain.Entities.Common;
using GameOnTonight.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GameOnTonight.Infrastructure.Configurations;

public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.Ignore(x => x.DomainErrors);
        builder.Ignore(x => x.HasErrors);
    }
}