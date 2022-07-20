using System;

using YourBrand.Identity;
using YourBrand.IdentityService.Application.Common.Interfaces;
using YourBrand.IdentityService.Services;

namespace YourBrand.IdentityService;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddIdentityServices();

        services.AddScoped<IEventPublisher, EventPublisher>();

        return services;
    }
}