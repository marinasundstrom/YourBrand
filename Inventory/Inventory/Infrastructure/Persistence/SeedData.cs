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

                var t1 = new Item("TS-B-S", "T-Shirt Blue Small", clothes.Id, "pc");
                var t2 = new Item("TS-B-M", "T-Shirt Blue Medium", clothes.Id, "pc");
                var t3 = new Item("TS-B-L", "T-Shirt Blue Large", clothes.Id, "pc");

                context.Items.Add(t1);
                context.Items.Add(t2);
                context.Items.Add(t3);

                await context.SaveChangesAsync();

                var site = new Site("main-site", "Main site");
                context.Sites.Add(site);

                var warehouse = new Warehouse("main-warehouse", "Main warehouse", site.Id);
                context.Warehouses.Add(warehouse);

                await context.SaveChangesAsync();

                var wt1 = new WarehouseItem(t1.Id, warehouse.Id, "1-2-3", 100);
                var wt2 = new WarehouseItem(t2.Id, warehouse.Id,"1-2-4", 100);
                var wt3 = new WarehouseItem(t3.Id, warehouse.Id, "1-2-5", 100);

                context.WarehouseItems.Add(wt1);
                context.WarehouseItems.Add(wt2);
                context.WarehouseItems.Add(wt3);

                await context.SaveChangesAsync();
            }
        }
    }
}