using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Catalog.Infrastructure.Persistence.Configurations;

public class AttributeConfiguration : IEntityTypeConfiguration<Domain.Entities.Attribute>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Attribute> builder)
    {
        builder.ToTable("Attributes");

        builder
            .HasMany(p => p.Values)
            .WithOne(p => p.Attribute);
    }
}
