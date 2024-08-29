using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Invoicing.Domain.Entities;

namespace YourBrand.Invoicing.Infrastructure.Persistence.Configurations;

public class InvoiceStatusConfiguration : IEntityTypeConfiguration<InvoiceStatus>
{
    public void Configure(EntityTypeBuilder<InvoiceStatus> builder)
    {
        builder.ToTable("InvoiceStatuses");

        builder.HasKey(o => new { o.OrganizationId, o.Id });
    }
}