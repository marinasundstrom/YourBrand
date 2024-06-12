using StackExchange.Redis;

namespace ChatApp.Web.Extensions;

public static class CachingExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMemoryCache();

        IConnectionMultiplexer? connection = null;

        var connectionString = configuration.GetConnectionString("redis") ?? "localhost";
        var c = ConfigurationOptions.Parse(connectionString, true);

        connection = ConnectionMultiplexer.Connect(c);

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            /*
            var connectionString = builder.Configuration.GetConnectionString("redis");
            var configuration = ConfigurationOptions.Parse(connectionString, true);
            return connection = ConnectionMultiplexer.Connect(configuration); */

            return connection;
        });

        services.AddStackExchangeRedisCache(options =>
        {
            //o.Configuration = builder.Configuration.GetConnectionString("redis");
            options.ConnectionMultiplexerFactory = () =>
            {
                return Task.FromResult(connection!);
            };
        });

        return services;
    }


}
