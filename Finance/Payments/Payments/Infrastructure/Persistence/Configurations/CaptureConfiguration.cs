using YourBrand.Payments.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace YourBrand.Payments.Infrastructure.Persistence.Configurations;

public class CaptureConfiguration : IEntityTypeConfiguration<Capture>
{
    public void Configure(EntityTypeBuilder<Capture> builder)
    {
        builder.ToTable("Captures");

        builder.Ignore(e => e.DomainEvents);
    }
}