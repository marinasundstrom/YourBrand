
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Orders.Infrastructure.Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");

        builder.HasAlternateKey(o => new { o.OrganizationId, o.SubscriptionNo });

        builder.HasIndex(x => x.TenantId);

        builder.HasOne(o => o.Type).WithMany()
    .HasForeignKey(o => new { o.OrganizationId, o.TypeId });

        builder.HasOne(o => o.Status).WithMany()
            .HasForeignKey(o => new { o.OrganizationId, o.StatusId });

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

        builder.OwnsOne(s => s.Schedule);

        builder.Property(p => p.CancellationFinalizationPeriod).HasConversion(new TimeSpanToTicksConverter());
    }
}