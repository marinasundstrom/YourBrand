using Catalog.Domain.Entities;
using Catalog.Infrastructure.Persistence;

using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<CatalogContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        /*
        if (!context.Items.Any())
        {
            context.Items.AddRange(new Item[] {
                new Item("Hat", "Green hat")
            });

            await context.SaveChangesAsync();
        }
        */
    }
}