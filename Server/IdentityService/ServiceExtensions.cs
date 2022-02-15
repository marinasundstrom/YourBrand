using System;

using Skynet.IdentityService.Application;
using Skynet.IdentityService.Application.Common.Interfaces;
using Skynet.IdentityService.Infrastructure;
using Skynet.IdentityService.Services;

namespace Skynet.IdentityService;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        return services;
    }
}