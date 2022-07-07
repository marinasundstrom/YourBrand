using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Infrastructure.Persistence.Configurations;

public class VariantValueConfiguration : IEntityTypeConfiguration<VariantValue>
{
    public void Configure(EntityTypeBuilder<VariantValue> builder)
    {
        builder.ToTable("VariantValues");

        builder.HasOne(m => m.Value).WithMany(m => m.VariantValues).OnDelete(DeleteBehavior.NoAction);
    }
}