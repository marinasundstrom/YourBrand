using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Persistence.Configurations;

public class ProducerConfiguration : IEntityTypeConfiguration<Producer>
{
    public void Configure(EntityTypeBuilder<Producer> builder)
    {
        builder.ToTable("Producers");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.Handle);

        builder.HasMany<Brand>().WithOne(x => x.Producer)
            .HasForeignKey(x => new { x.OrganizationId, x.ProducerId });
    }
}