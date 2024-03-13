using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Portal.Widgets;

public static class ServiceExtensions
{
    public static IServiceCollection AddWidgets(this IServiceCollection services)
    {
        services.AddScoped<IWidgetService, WidgetService>();

        return services;
    }
}