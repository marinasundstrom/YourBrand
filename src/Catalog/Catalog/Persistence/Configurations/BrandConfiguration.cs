using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.Handle);

        builder.HasOne(o => o.Parent).WithMany(x => x.SubBrands)
            .HasForeignKey(o => new { o.OrganizationId, o.ParentId });

        /* builder.HasMany(o => o.SubBrands).WithOne(x => x.Parent)
            .HasForeignKey(o => new { o.OrganizationId, o.ParentId })
            .OnDelete(DeleteBehavior.Cascade); */
    }
}