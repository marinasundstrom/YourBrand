using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Payments.Domain.Entities;

namespace YourBrand.Payments.Infrastructure.Persistence.Configurations;

public class CaptureConfiguration : IEntityTypeConfiguration<Capture>
{
    public void Configure(EntityTypeBuilder<Capture> builder)
    {
        builder.ToTable("Captures");
    }
}