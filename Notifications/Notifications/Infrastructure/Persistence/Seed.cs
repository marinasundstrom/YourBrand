using Microsoft.Extensions.DependencyInjection;

using YourBrand.Notifications.Domain.Entities;
using YourBrand.Notifications.Infrastructure.Persistence;

namespace YourBrand.Notifications.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<WorkerContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}