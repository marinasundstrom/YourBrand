using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Infrastructure.Persistence;

public class SeedData
{
    public static async Task EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();

            var context = scope.ServiceProvider.GetRequiredService<InventoryContext>();
            await context.Database.EnsureDeletedAsync();
            //context.Database.Migrate();
            await context.Database.EnsureCreatedAsync();

            if (!context.Items.Any())
            {
                var clothes = new ItemGroup("Clothes");
                context.ItemGroups.Add(clothes);

                var t1 = new Item("TS-B-S", "T-Shirt Blue Small", Domain.Enums.ItemType.Inventory, "Foo1", clothes.Id, "pc");
                var t2 = new Item("TS-B-M", "T-Shirt Blue Medium", Domain.Enums.ItemType.Inventory, "Foo2", clothes.Id, "pc");
                var t3 = new Item("TS-B-L", "T-Shirt Blue Large", Domain.Enums.ItemType.Inventory, "Foo3", clothes.Id, "pc");

                context.Items.Add(t1);
                context.Items.Add(t2);
                context.Items.Add(t3);

                await context.SaveChangesAsync();

                var site = new Site("main-site", "Main Site");
                context.Sites.Add(site);

                var warehouse = new Warehouse("main-warehouse", "Main Warehouse", site);
                context.Warehouses.Add(warehouse);

                await context.SaveChangesAsync();

                var wt1 = new WarehouseItem(t1, warehouse, "1-2-3", 100);
                var wt2 = new WarehouseItem(t2, warehouse, "1-2-4", 100);
                var wt3 = new WarehouseItem(t3, warehouse, "1-2-5", 100);

                context.WarehouseItems.Add(wt1);
                context.WarehouseItems.Add(wt2);
                context.WarehouseItems.Add(wt3);

                await context.SaveChangesAsync();

                var supplier = new Supplier("default-supplier", "Default Supplier");
                context.Suppliers.Add(supplier);

                await context.SaveChangesAsync();

                var supplierItem1 = new SupplierItem(supplier, t1, "TS-B-S", 5.75m, 7);
                var supplierItem2 = new SupplierItem(supplier, t2, "TS-B-M", 5.75m, 7);
                var supplierItem3 = new SupplierItem(supplier, t3, "TS-B-L", 5.75m, 7);

                context.SupplierItems.AddRange(supplierItem1, supplierItem2, supplierItem3);

                await context.SaveChangesAsync();

                var supplierOrder = new SupplierOrder(supplier, "PO-1001", DateTime.UtcNow, DateTime.UtcNow.AddDays(10));
                supplierOrder.AddLine(supplierItem1, 50, DateTime.UtcNow.AddDays(10));
                supplierOrder.AddLine(supplierItem2, 50, DateTime.UtcNow.AddDays(10));

                context.SupplierOrders.Add(supplierOrder);

                await context.SaveChangesAsync();

                var shop1site = new Site("shop-1", "Shop 1");
                context.Sites.Add(shop1site);

                var shop1warehouse = new Warehouse("shop-1-main", "Main", shop1site);
                context.Warehouses.Add(shop1warehouse);

                var shop2site = new Site("shop-2", "Shop 2");
                context.Sites.Add(shop2site);

                var shop2warehouse = new Warehouse("shop-2-main", "Main", shop2site);
                context.Warehouses.Add(shop2warehouse);

                await context.SaveChangesAsync();
            }
        }
    }
}