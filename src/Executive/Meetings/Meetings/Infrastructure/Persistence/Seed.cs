using YourBrand.Tenancy;
using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Meetings.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ISettableTenantContext>();
        tenantContext.SetTenantId(TenantConstants.TenantId);

        using var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        await context.SaveChangesAsync();
    }
}