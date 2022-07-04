
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using YourBrand.Orders.Domain.Entities;

namespace YourBrand.Orders.Infrastructure.Persistence.Configurations
{
    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.Ignore(e => e.DomainEvents);

            builder.HasQueryFilter(e => e.Deleted == null);

            builder.HasOne(s => s.Order!)
              .WithOne(oi => oi.Subscription!)
              .HasForeignKey<Subscription>(s => s.OrderId);

            builder.HasOne(s => s.OrderItem!)
                .WithOne(oi => oi.Subscription!)
                .HasForeignKey<Subscription>(s => s.OrderItemId);
        }
    }
}