using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class OptionGroupConfiguration : IEntityTypeConfiguration<OptionGroup>
{
    public void Configure(EntityTypeBuilder<OptionGroup> builder)
    {
        builder.ToTable("OptionGroups");

        builder.HasIndex(x => x.TenantId);

        builder
            .HasOne(x => x.Product)
            .WithMany(x => x.OptionGroups)
            .OnDelete(DeleteBehavior.ClientNoAction);
    }
}