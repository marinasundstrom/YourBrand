using System;

using YourBrand.UserManagement.Application.Common.Interfaces;
using YourBrand.UserManagement.Services;
using YourBrand.Identity;

namespace YourBrand.UserManagement;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddIdentityServices();

        services.AddScoped<IEventPublisher, EventPublisher>();

        return services;
    }
}