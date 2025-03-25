using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Persistence.Configurations;

public sealed class OrderItemOptionConfiguration : IEntityTypeConfiguration<OrderItemOption>
{
    public void Configure(EntityTypeBuilder<OrderItemOption> builder)
    {
        builder.ToTable("OrderItemOptions");

        builder.HasKey(o => new { o.OrganizationId, o.OrderId, o.OrderItemId, o.Id });

        builder.Ignore(e => e.DomainEvents);
    }
}