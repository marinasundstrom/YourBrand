using System;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Skynet.IdentityService.Application.Users.Commands;

namespace Skynet.IdentityService.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(CreateUserCommand));

        return services;
    }
}