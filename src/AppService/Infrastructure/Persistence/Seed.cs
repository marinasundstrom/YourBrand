using Microsoft.Extensions.DependencyInjection;

using YourBrand.Application.Modules;
using YourBrand.Domain.Entities;

namespace YourBrand.Infrastructure.Persistence;

public static class Seed
{
    public static async Task SeedAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
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

        if (!context.Items.Any())
        {
            context.Items.Add(new Item("Item 1"));
            context.Items.Add(new Item("Item 2"));
            context.Items.Add(new Item("Item 3"));
            context.Items.Add(new Item("Foo"));
            context.Items.Add(new Item("Foobar"));

            await context.SaveChangesAsync();
        }

        var widgetArea = new WidgetArea("dashboard", "Dashboard");
        widgetArea.AddWidget(new Widget("analytics.engagements", null, null));
        widgetArea.AddWidget(new Widget("sample-widget2", null, null));

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
                Id = Guid.NewGuid(),
                Name = module.Name,
                Assembly = module.Assembly,
                Enabled = module.Enabled,
                Index = i++
            });
        }

        await context.SaveChangesAsync();

        context.Set<WidgetArea>().Add(new WidgetArea("dashboard", "Dashboard"));

        context.Widgets.Add(new Widget(Guid.NewGuid().ToString(), null, null)
        {
            WidgetId = "analytics.engagements",
            WidgetAreaId = "dashboard"
        });

        context.Widgets.Add(new Widget(Guid.NewGuid().ToString(), null, null)
        {
            WidgetId = "tickets.overview",
            WidgetAreaId = "dashboard"
        });

        context.Widgets.Add(new Widget(Guid.NewGuid().ToString(), null, null)
        {
            WidgetId = "orders.pendingOrders",
            WidgetAreaId = "dashboard"
        });

        await context.SaveChangesAsync();
    }
}