using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class AttributeValueConfiguration : IEntityTypeConfiguration<AttributeValue>
{
    public void Configure(EntityTypeBuilder<AttributeValue> builder)
    {
        builder.ToTable("AttributeValues");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasOne(o => o.Attribute).WithMany(x => x.Values)
            .HasForeignKey(o => new { o.OrganizationId, o.AttributeId });
    }
}