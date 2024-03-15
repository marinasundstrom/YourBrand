using YourBrand.Domain.Entities;
using YourBrand.Infrastructure.Persistence;

using Microsoft.Extensions.DependencyInjection;

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
            context.Users.Add(new User {
                Id = "api",
                FirstName = "API",
                LastName = "User",
                SSN = "213",
                Email = "test@foo.com",
                Hidden = true
            });

            await context.SaveChangesAsync();
        }

        var widgetArea = new WidgetArea("dashboard", "Dashboard");
        widgetArea.AddWidget(new Widget("analytics.engagements", null, null));
        widgetArea.AddWidget(new Widget("sample-widget2", null, null));

        context.Set<WidgetArea>().Add(widgetArea);

        await context.SaveChangesAsync();
    }
}