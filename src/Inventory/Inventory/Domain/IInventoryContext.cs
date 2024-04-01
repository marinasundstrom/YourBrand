using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Domain;

public interface IInventoryContext
{
    DbSet<Site> Sites { get; }
    DbSet<Warehouse> Warehouses { get; }
    DbSet<WarehouseItem> WarehouseItems { get; }
    DbSet<Location> Locations { get; }
    DbSet<ItemGroup> ItemGroups { get; }
    DbSet<Item> Items { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}