
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.ApiKeys.Domain.Entities;

namespace YourBrand.ApiKeys.Infrastructure.Persistence.Configurations;

sealed class ResourceConfiguration : IEntityTypeConfiguration<Resource>
{
    public void Configure(EntityTypeBuilder<Resource> builder)
    {
        builder.ToTable("Resources");
        builder.HasQueryFilter(i => i.Deleted == null);

        builder.Ignore(i => i.DomainEvents);
    }
}