using YourBrand.Warehouse.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Warehouse.Domain;

public interface IWarehouseContext
{
    DbSet<Item> Items { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}