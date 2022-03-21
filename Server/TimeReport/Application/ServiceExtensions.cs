using System;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourCompany.TimeReport.Application.Expenses.Queries;

namespace YourCompany.TimeReport.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(GetExpensesQuery));

        return services;
    }
}