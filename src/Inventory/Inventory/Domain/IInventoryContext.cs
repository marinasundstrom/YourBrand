using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Domain;

public interface IInventoryContext
{
    DbSet<Site> Sites { get; }
    DbSet<Warehouse> Warehouses { get; }
    DbSet<WarehouseItem> WarehouseItems { get; }
    DbSet<ItemGroup> ItemGroups { get; }
    DbSet<Item> Items { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<SupplierItem> SupplierItems { get; }
    DbSet<SupplierOrder> SupplierOrders { get; }
    DbSet<SupplierOrderLine> SupplierOrderLines { get; }
    DbSet<Shipment> Shipments { get; }
    DbSet<ShipmentItem> ShipmentItems { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}