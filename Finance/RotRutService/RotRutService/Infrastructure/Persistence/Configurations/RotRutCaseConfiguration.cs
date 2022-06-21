using YourBrand.RotRutService.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.RotRutService.Infrastructure.Persistence.Configurations;

public class RotRutCaseConfiguration : IEntityTypeConfiguration<RotRutCase>
{
    public void Configure(EntityTypeBuilder<RotRutCase> builder)
    {
        builder.ToTable("RotRutCases");

        builder.Ignore(x => x.Rot);
        builder.Ignore(x => x.Rut);
    }
}