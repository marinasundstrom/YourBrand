using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Persistence.Configurations;

public sealed class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");

        builder.HasKey(o => new { o.OrganizationId, o.OrderId, o.Id });

        builder.HasIndex(x => x.TenantId);

        // builder.HasOne(s => s.Subscription!)
        //     .WithOne()
        //     .HasForeignKey<OrderItem>(s => s.SubscriptionId);

        builder.Ignore(e => e.DomainEvents);

    }
}