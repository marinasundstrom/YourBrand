using YourBrand.Orders.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Orders.Domain;

public interface IOrdersContext
{
    DbSet<Order> Orders { get; }
    DbSet<OrderStatus> OrderStatuses { get; }
    DbSet<OrderTotals> OrderTotals { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<OrderCharge> OrderCharges { get; }
    DbSet<OrderDiscount> OrderDiscounts { get; }

    DbSet<Subscription> Subscriptions { get; }
    DbSet<SubscriptionPlan> SubscriptionPlans { get; }

    DbSet<PriceList> PriceLists { get; }
    DbSet<PriceListItem> PriceListItems { get; }

    DbSet<CustomField> CustomFields { get; }
    DbSet<CustomFieldDefinition> CustomFieldDefinitions { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}