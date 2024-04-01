
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Orders.Infrastructure.Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        //builder.HasAlternateKey(o => new { o.OrganizationId, o.SubscriptionNo });

        builder.HasIndex(x => x.TenantId);

        builder.Ignore(e => e.DomainEvents);

        builder.HasQueryFilter(e => e.Deleted == null);

        builder.HasOne(s => s.Order!)
            .WithOne()
            .HasForeignKey<Subscription>(s => s.OrderId);

        builder.HasOne(s => s.OrderItem!)
            .WithOne()
            .HasForeignKey<Subscription>(s => s.OrderItemId);

        builder.HasMany(s => s.Orders!)
            .WithOne(x => x.Subscription)
            .HasForeignKey(s => s.SubscriptionId);

        builder.HasMany(s => s.OrderItems!)
            .WithOne(x => x.Subscription)
            .HasForeignKey(s => s.SubscriptionId);
    }
}