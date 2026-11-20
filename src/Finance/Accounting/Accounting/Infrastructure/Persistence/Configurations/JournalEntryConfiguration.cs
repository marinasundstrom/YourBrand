using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Accounting.Domain.Entities;

namespace YourBrand.Accounting.Infrastructure.Persistence.Configurations;

public class VerificationConfiguration : IEntityTypeConfiguration<Verification>
{
    public void Configure(EntityTypeBuilder<Verification> builder)
    {
        builder.ToTable("Verifications");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.HasMany(x => x.Entries)
            .WithOne(x => x.Verification)
            .HasForeignKey(x => new { x.OrganizationId, x.VerificationId });

        builder.HasMany(x => x.SupportingDocuments)
            .WithOne(x => x.Verification)
            .HasForeignKey(x => new { x.OrganizationId, x.VerificationId });
    }
}