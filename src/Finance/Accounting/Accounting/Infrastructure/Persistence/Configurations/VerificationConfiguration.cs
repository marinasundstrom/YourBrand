using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Accounting.Domain.Entities;

namespace YourBrand.Accounting.Infrastructure.Persistence.Configurations;

public class SupportingDocumentConfiguration : IEntityTypeConfiguration<SupportingDocument>
{
    public void Configure(EntityTypeBuilder<SupportingDocument> builder)
    {
        builder.ToTable("SupportingDocuments");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        builder.HasIndex(x => x.TenantId);
    }
}