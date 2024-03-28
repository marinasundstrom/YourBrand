using System;

using YourBrand.Identity;
using YourBrand.Notifications.Application;
using YourBrand.Notifications.Application.Common.Interfaces;
using YourBrand.Notifications.Infrastructure;
using YourBrand.Notifications.Services;

namespace YourBrand.Notifications;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddIdentityServices();

        services.AddScoped<INotifier, Notifier>();

        services.AddScoped<INotificationPublisher, NotificationPublisher>();

        return services;
    }
}