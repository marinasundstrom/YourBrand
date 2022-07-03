using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Infrastructure.Persistence.Configurations;

public class OptionConfiguration : IEntityTypeConfiguration<Option>
{
    public void Configure(EntityTypeBuilder<Option> builder)
    {
        builder.ToTable("Options");

        builder
            .HasMany(p => p.Values)
            .WithOne(p => p.Option);

        builder.HasOne(p => p.DefaultValue);
    }
}
