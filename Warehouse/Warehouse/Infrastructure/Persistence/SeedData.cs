using YourBrand.Warehouse.Domain.Entities;

namespace YourBrand.Warehouse.Infrastructure.Persistence;

public class SeedData
{
    public static async Task EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();

            var context = scope.ServiceProvider.GetRequiredService<WarehouseContext>();
            await context.Database.EnsureDeletedAsync();
            //context.Database.Migrate();
            await context.Database.EnsureCreatedAsync();

            if (!context.Items.Any())
            {
                var person = new Item("TS-B-L", "T-Short Blue Larger", 50);

                context.Items.Add(person);

                await context.SaveChangesAsync();
            }
        }
    }
}