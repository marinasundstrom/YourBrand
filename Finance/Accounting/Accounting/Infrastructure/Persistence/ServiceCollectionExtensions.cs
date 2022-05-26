using YourBrand.Accounting.Application.Common.Interfaces;
using YourBrand.Accounting.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Accounting.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AccountingContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("mssql", "Accounting") ?? configuration.GetConnectionString("DefaultConnection"), o => o.EnableRetryOnFailure());
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IAccountingContext>(sp => sp.GetRequiredService<AccountingContext>());

        return services;
    }
}