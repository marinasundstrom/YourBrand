using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Domain.Entities;

namespace YourBrand.Infrastructure.Persistence.Configurations;

public sealed class BrandProfileConfiguration : IEntityTypeConfiguration<BrandProfile>
{
    public void Configure(EntityTypeBuilder<BrandProfile> builder)
    {
        builder.ToTable("BrandProfiles");

        builder.HasOne(x => x.Theme);

        builder.Navigation(x => x.Theme).AutoInclude();
    }
}
