using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Domain;

public interface ISalesContext
{
    DbSet<OrderStatus> OrderStatuses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}