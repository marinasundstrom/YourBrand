using YourBrand.Tenancy;

namespace YourBrand.Notifications.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedAsync(this IServiceProvider services, string? tenantId = null)
    {
        using var scope = services.CreateScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ISettableTenantContext>();
        tenantContext.SetTenantId(string.IsNullOrWhiteSpace(tenantId) ? TenantConstants.TenantId : tenantId);

        using var context = scope.ServiceProvider.GetRequiredService<NotificationsContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}