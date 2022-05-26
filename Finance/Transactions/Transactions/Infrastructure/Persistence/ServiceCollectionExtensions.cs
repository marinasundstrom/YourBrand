using Microsoft.EntityFrameworkCore;

using YourBrand.Transactions.Domain;

namespace YourBrand.Transactions.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = configuration.GetConnectionString(ConnectionStringKey, "Transactions")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<TransactionsContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure());
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<ITransactionsContext>(sp => sp.GetRequiredService<TransactionsContext>());

        return services;
    }
}