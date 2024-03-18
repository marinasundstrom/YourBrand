using System;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.UserManagement.Application.Users.Commands;

namespace YourBrand.UserManagement.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(CreateUserCommand)));

        return services;
    }
}