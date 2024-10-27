using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Persistence.Configurations;

public sealed class OrderStatusConfiguration : IEntityTypeConfiguration<OrderStatus>
{
    public void Configure(EntityTypeBuilder<OrderStatus> builder)
    {
        builder.ToTable("OrderStatuses");

        builder.HasKey(x => new { x.OrganizationId, x.Id });

        //builder.HasAlternateKey(o => new { o.OrganizationId, o.Id });

        builder.HasIndex(x => x.TenantId);

        builder.Ignore(e => e.DomainEvents);

    }
}
