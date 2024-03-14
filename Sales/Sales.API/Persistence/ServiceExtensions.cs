using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Sales.API.Features.OrderManagement.Repositories;
using YourBrand.Sales.API.Persistence.Interceptors;
using YourBrand.Sales.API.Persistence.Repositories;
using YourBrand.Sales.API.Persistence.Repositories.Mocks;

using YourBrand.Domain.Persistence;
using YourBrand.Domain.Persistence.Interceptors;

namespace YourBrand.Sales.API.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = Infrastructure.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, "Orders")
            ?? configuration.GetValue<string>("yourbrand:sales-svc:db:connectionstring");

        services.AddDomainPersistence<SalesContext>(configuration);

        services.AddDbContext<SalesContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString!, o => o.EnableRetryOnFailure());

            options.AddInterceptors(
                sp.GetRequiredService<OutboxSaveChangesInterceptor>(),
                sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

#if DEBUG
            options
                //.LogTo(Console.WriteLine)
                .EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<ISalesContext>(sp => sp.GetRequiredService<SalesContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        RegisterRepositories(services);

        return services;
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        // TODO: Automate this

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<SalesContext>());

        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
    }
}