using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Domain;
using YourBrand.Documents.Infrastructure.Persistence.Interceptors;

namespace YourBrand.Documents.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = YourBrand.Documents.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, "Documents")
            ?? configuration.GetConnectionString("DefaultConnection");

        Console.WriteLine(connectionString);

        services.AddDbContext<DocumentsContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure());
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IDocumentsContext>(sp => sp.GetRequiredService<DocumentsContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        return services;
    }
}