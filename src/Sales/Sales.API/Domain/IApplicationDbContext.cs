using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Domain;

public interface ISalesContext
{
    DbSet<OrderType> OrderTypes { get; }

    DbSet<OrderStatus> OrderStatuses { get; }

    DbSet<SubscriptionType> SubscriptionTypes { get; }

    DbSet<SubscriptionStatus> SubscriptionStatuses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}