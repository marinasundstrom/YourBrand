using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Domain.Entities;

namespace YourBrand.Infrastructure.Persistence.Configurations;

public sealed class ThemeConfiguration : IEntityTypeConfiguration<Theme>
{
    public void Configure(EntityTypeBuilder<Theme> builder)
    {
        builder.ToTable("Themes");

        builder.OwnsOne(x => x.ColorSchemes, x =>
        {
            x.ToJson();
            
            x.OwnsOne(x => x.Light);
            x.OwnsOne(x => x.Dark);
        });
    }
}