using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Catalog.Persistence.Configurations;

public class AttributeConfiguration : IEntityTypeConfiguration<Domain.Entities.Attribute>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Attribute> builder)
    {
        builder.ToTable("Attributes");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder
            .HasMany(p => p.Values)
            .WithOne(p => p.Attribute);

        builder.HasOne(o => o.ProductCategory).WithMany(x => x.Attributes)
            .HasForeignKey(o => new { o.OrganizationId, o.ProductCategoryId });

        builder
            .HasMany(x => x.ProductAttributes)
            .WithOne(x => x.Attribute)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(o => o.Group).WithMany(x => x.Attributes)
            .HasForeignKey(o => new { o.OrganizationId, o.GroupId })
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}