
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.ApiKeys.Domain.Entities;

namespace YourBrand.ApiKeys.Infrastructure.Persistence.Configurations;

sealed class ItemConfiguration : IEntityTypeConfiguration<ApiKey>
{
    public void Configure(EntityTypeBuilder<ApiKey> builder)
    {
        builder.ToTable("ApiKeys");
        builder.HasQueryFilter(i => i.Deleted == null);

        builder.HasIndex(nameof(ApiKey.Key));

        builder.Ignore(i => i.DomainEvents);
    }
}