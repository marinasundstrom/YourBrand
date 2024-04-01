using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.RotRutService.Domain.Entities;

namespace YourBrand.RotRutService.Infrastructure.Persistence.Configurations;

public class RotRutCaseConfiguration : IEntityTypeConfiguration<RotRutCase>
{
    public void Configure(EntityTypeBuilder<RotRutCase> builder)
    {
        builder.ToTable("RotRutCases");

        builder.OwnsOne(x => x.Rot);
        builder.Navigation(x => x.Rot).IsRequired();
        builder.OwnsOne(x => x.Rut);
        builder.Navigation(x => x.Rut).IsRequired();
    }
}