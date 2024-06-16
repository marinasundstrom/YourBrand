namespace YourBrand;

using Microsoft.Extensions.DependencyInjection;

using YourBrand.Notifications;

public static class ServiceExtensions
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }
}