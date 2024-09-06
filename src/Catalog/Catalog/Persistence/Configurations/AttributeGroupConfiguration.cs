﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class AttributeGroupConfiguration : IEntityTypeConfiguration<AttributeGroup>
{
    public void Configure(EntityTypeBuilder<AttributeGroup> builder)
    {
        builder.ToTable("AttributeGroups");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasOne(o => o.Product).WithMany(x => x.AttributeGroups)
            .HasForeignKey(o => new { o.OrganizationId, o.ProductId });
    }
}