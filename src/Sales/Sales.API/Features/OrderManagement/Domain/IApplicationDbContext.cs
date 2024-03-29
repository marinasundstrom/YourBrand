using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using YourBrand.Sales.Features.OrderManagement.Domain.Entities;

namespace YourBrand.Sales.Features.OrderManagement.Domain;

public interface ISalesContext
{
    DbSet<OrderStatus> OrderStatuses { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}