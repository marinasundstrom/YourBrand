using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class OptionGroupConfiguration : IEntityTypeConfiguration<OptionGroup>
{
    public void Configure(EntityTypeBuilder<OptionGroup> builder)
    {
        builder.ToTable("OptionGroups");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasOne(o => o.Product).WithMany(x => x.OptionGroups)
            .HasForeignKey(o => new { o.OrganizationId, o.ProductId })
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}