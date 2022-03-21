using System;

using YourCompany.IdentityService.Application;
using YourCompany.IdentityService.Application.Common.Interfaces;
using YourCompany.IdentityService.Infrastructure;
using YourCompany.IdentityService.Services;

namespace YourCompany.IdentityService;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        services.AddScoped<IEventPublisher, EventPublisher>();

        return services;
    }
}