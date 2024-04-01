using YourBrand.Carts.API.Domain.Entities;

namespace YourBrand.Carts.API.Persistence;

public static class Seed
{
    public static async Task SeedData(CartsContext context, IConfiguration configuration)
    {
        //await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        context.Carts.Add(new Cart("test", "Test"));

        await context.SaveChangesAsync();
    }
}