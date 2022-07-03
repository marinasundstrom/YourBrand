using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Infrastructure.Persistence.Configurations;

public class VariantValueConfiguration : IEntityTypeConfiguration<VariantValue>
{
    public void Configure(EntityTypeBuilder<VariantValue> builder)
    {
        builder.ToTable("VariantValues");

        builder.HasOne(m => m.Value).WithMany(m => m.VariantValues).OnDelete(DeleteBehavior.NoAction);
    }
}