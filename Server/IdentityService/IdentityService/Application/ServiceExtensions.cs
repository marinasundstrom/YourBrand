using System;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourCompany.IdentityService.Application.Users.Commands;

namespace YourCompany.IdentityService.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(CreateUserCommand));

        return services;
    }
}