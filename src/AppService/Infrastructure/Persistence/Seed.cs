using Microsoft.Extensions.DependencyInjection;

using YourBrand.Application.Modules;
using YourBrand.Domain.Entities;
using YourBrand.Tenancy;

namespace YourBrand.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var tenantContext = scope.ServiceProvider.GetRequiredService<ISettableTenantContext>();
        tenantContext.SetTenantId(TenantConstants.TenantId);

        using var context = scope.ServiceProvider.GetRequiredService<AppServiceContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();

        if (!context.Users.Any())
        {
            context.Users.Add(new User
            {
                Id = "api",
                FirstName = "API",
                LastName = "User",
                SSN = "213",
                Email = "test@foo.com",
                Hidden = true
            });

            await context.SaveChangesAsync();
        }

        if (!context.SearchResultItems.Any())
        {
            context.SearchResultItems.Add(new SearchResultItem("Item 1"));
            context.SearchResultItems.Add(new SearchResultItem("Item 2"));
            context.SearchResultItems.Add(new SearchResultItem("Item 3"));
            context.SearchResultItems.Add(new SearchResultItem("Foo"));
            context.SearchResultItems.Add(new SearchResultItem("Foobar"));

            await context.SaveChangesAsync();
        }

        var widgetArea = new WidgetArea("dashboard", "Dashboard");

        context.Set<WidgetArea>().Add(widgetArea);

        await context.SaveChangesAsync();

        var modules = System.Text.Json.JsonSerializer.Deserialize<IEnumerable<ModuleDto>>(
               await File.ReadAllTextAsync("modules.json")
           )!;

        int i = 1;

        foreach (var module in modules)
        {
            context.Modules.Add(new Module
            {
                Id = module.Id,
                Name = module.Name,
                Assembly = module.Assembly,
                Enabled = module.Enabled,
                Index = i++,
                DependantOn = module.DependantOn?.ToList() ?? new List<string>()
            });
        }

        await context.SaveChangesAsync();

        context.Widgets.Add(new Widget(Guid.NewGuid().ToString(), null)
        {
            WidgetId = "analytics.engagements",
            WidgetAreaId = "dashboard"
        });

        context.Widgets.Add(new Widget(Guid.NewGuid().ToString(), null)
        {
            WidgetId = "tickets.overview",
            WidgetAreaId = "dashboard"
        });

        context.Widgets.Add(new Widget(Guid.NewGuid().ToString(), null)
        {
            WidgetId = "orders.pendingOrders",
            WidgetAreaId = "dashboard"
        });

        await context.SaveChangesAsync();

        context.BrandProfiles.Add(new BrandProfile(Guid.NewGuid().ToString(), null)
        {
            Name = "Default",
            BackgroundColor = "rgb(248, 249, 250)",
            AppbarBackgroundColor = "#137cdf",
            PrimaryColor = "#4892d7",
            SecondaryColor = "#ee1919"
        });

        context.BrandProfiles.Add(new BrandProfile(Guid.NewGuid().ToString(), null)
        {
            Name = "ACME",
            BackgroundColor = null,
            AppbarBackgroundColor = "#1b3b5d",
            PrimaryColor = "#1c4168",
            SecondaryColor = "#ee1919"
        });

        await context.SaveChangesAsync();
    }
}