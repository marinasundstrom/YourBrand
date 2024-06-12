using YourBrand.ChatApp.Infrastructure;

namespace YourBrand.ChatApp.Infrastructure;

public static class ConfigurationExtensions
{
    public static string? GetConnectionString(this IConfiguration configuration, string name, string database)
    {
        var connectionString = configuration.GetConnectionString(name);
        if (connectionString is null)
        {
            return null;
        }
        return $"{connectionString};Database={database}";
    }
}