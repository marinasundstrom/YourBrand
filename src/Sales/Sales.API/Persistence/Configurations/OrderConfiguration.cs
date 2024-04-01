using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasIndex(x => x.TenantId);

        builder.HasAlternateKey(o => new { o.OrganizationId , o.OrderNo });

        builder.HasMany(order => order.Items)
            .WithOne(orderItem => orderItem.Order)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.OwnsOne(x => x.Customer);

        builder.OwnsOne(x => x.BillingDetails, x => x.OwnsOne(z => z.Address));

        builder.OwnsOne(x => x.ShippingDetails, x => x.OwnsOne(z => z.Address));

        builder.OwnsMany(x => x.VatAmounts, x => x.ToJson());

        builder.OwnsMany(x => x.Discounts, x => x.ToJson());

        // builder.HasOne(s => s.Subscription!)
        //     .WithOne()
        //     .HasForeignKey<Order>(s => s.SubscriptionId);

        builder.Ignore(e => e.DomainEvents);

    }
}