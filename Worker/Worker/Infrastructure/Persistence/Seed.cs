using Microsoft.Extensions.DependencyInjection;

using Worker.Domain.Entities;
using Worker.Infrastructure.Persistence;

namespace Worker.Infrastructure.Persistence;

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