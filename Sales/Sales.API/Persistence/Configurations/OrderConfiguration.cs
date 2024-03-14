using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;

namespace YourBrand.Sales.API.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        //builder.HasKey(o => new { o.CompanyId , o.OrderNo });

        builder.HasMany(order => order.Items)
            .WithOne(orderItem => orderItem.Order)
            .IsRequired()
            .OnDelete(DeleteBehavior.ClientCascade);

        builder.OwnsOne(x => x.BillingDetails, x => x.OwnsOne(z => z.Address));

        builder.OwnsOne(x => x.ShippingDetails, x => x.OwnsOne(z => z.Address));

        builder.OwnsMany(x => x.VatAmounts, x => x.ToJson());

        builder.OwnsMany(x => x.Discounts, x => x.ToJson());
    }
}