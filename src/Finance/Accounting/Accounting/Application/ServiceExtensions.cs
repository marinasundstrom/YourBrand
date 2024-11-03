using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Accounting.Application.Accounts.Queries;
using YourBrand.Accounting.Domain.Entities;

namespace YourBrand.Accounting.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(GetAccountQuery)));

        services.AddScoped<ILedgerEntryIdGenerator, LedgerEntryIdGenerator>();

        return services;
    }
}