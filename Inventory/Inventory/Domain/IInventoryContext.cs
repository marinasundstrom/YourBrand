using YourBrand.Inventory.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Inventory.Domain;

public interface IInventoryContext
{
    DbSet<Site> Sites { get; }
    DbSet<Warehouse> Warehouse { get; }
    DbSet<Location> Locations { get; }
    DbSet<ItemGroup> ItemGroups { get; }
    DbSet<Item> Items { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}