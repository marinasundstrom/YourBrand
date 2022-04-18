using System;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.TimeReport.Application.Projects.Expenses.Queries;

namespace YourBrand.TimeReport.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(GetExpensesQuery));

        return services;
    }
}