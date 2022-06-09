using System;

using YourBrand.IdentityService.Application.Common.Interfaces;
using YourBrand.IdentityService.Services;

namespace YourBrand.IdentityService;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IEventPublisher, EventPublisher>();

        return services;
    }
}