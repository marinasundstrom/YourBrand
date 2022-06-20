using System;

using Worker.Application;
using Worker.Application.Common.Interfaces;
using Worker.Infrastructure;
using Worker.Services;

namespace Worker;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<INotifier, Notifier>();

        services.AddScoped<INotificationPublisher, NotificationPublisher>();

        return services;
    }
}