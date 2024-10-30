using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => new { o.OrganizationId, o.Id });

        builder.HasIndex(o => new { o.OrganizationId, o.OrderNo });

        builder.HasOne(o => o.Type).WithMany()
            .HasForeignKey(o => new { o.OrganizationId, o.TypeId });

        builder.HasOne(o => o.Status).WithMany()
            .HasForeignKey(o => new { o.OrganizationId, o.StatusId });

        builder.HasIndex(x => x.TenantId);

        builder.HasMany(order => order.Items)
            .WithOne(orderItem => orderItem.Order)
            .HasForeignKey(o => new { o.OrganizationId, o.OrderId })
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.OwnsOne(x => x.Customer);

        builder.OwnsOne(x => x.Schedule);

        builder.OwnsOne(x => x.BillingDetails, x => x.OwnsOne(z => z.Address));

        builder.OwnsOne(x => x.ShippingDetails, x => x.OwnsOne(z => z.Address));

        builder.OwnsMany(x => x.VatAmounts, x => x.ToJson());

        builder.HasMany(order => order.Discounts)
             .WithOne()
             .HasForeignKey(o => new { o.OrganizationId, o.OrderItemId })
             .IsRequired()
             .OnDelete(DeleteBehavior.ClientCascade);

        builder.Navigation(x => x.Discounts).AutoInclude();

        // builder.HasOne(s => s.Subscription!)
        //     .WithOne()
        //     .HasForeignKey<Order>(s => s.SubscriptionId);

        builder.Ignore(e => e.DomainEvents);

    }
}