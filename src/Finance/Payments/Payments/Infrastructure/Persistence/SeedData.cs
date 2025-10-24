﻿using YourBrand.Tenancy;

namespace YourBrand.Payments.Infrastructure.Persistence;

public class SeedData
{
    public static async Task EnsureSeedData(WebApplication app, string tenantId)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedData>>();

            var tenantContext = scope.ServiceProvider.GetRequiredService<ISettableTenantContext>();
            tenantContext.SetTenantId(string.IsNullOrWhiteSpace(tenantId) ? TenantConstants.TenantId : tenantId);

            var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();
            await context.Database.EnsureDeletedAsync();
            //context.Database.Migrate();
            await context.Database.EnsureCreatedAsync();

            //await context.SaveChangesAsync();
        }
    }
}