using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Invoicing.Domain.Entities;

namespace YourBrand.Invoicing.Infrastructure.Persistence.Configurations;

public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");

        builder.HasKey(x => new { x.OrganizationId, x.InvoiceId, x.Id });

        builder.HasIndex(x => x.TenantId);

        builder.OwnsOne(x => x.DomesticService);

        builder.HasMany(order => order.Options)
            .WithOne()
            .HasForeignKey(o => new { o.OrganizationId, o.InvoiceId, o.InvoiceItemId })
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.Navigation(x => x.Options).AutoInclude();
    }
}