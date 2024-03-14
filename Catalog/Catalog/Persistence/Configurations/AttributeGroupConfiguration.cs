using YourBrand.Catalog.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Catalog.Persistence.Configurations;

public class AttributeGroupConfiguration : IEntityTypeConfiguration<AttributeGroup>
{
    public void Configure(EntityTypeBuilder<AttributeGroup> builder)
    {
        builder.ToTable("AttributeGroups");
    }
}