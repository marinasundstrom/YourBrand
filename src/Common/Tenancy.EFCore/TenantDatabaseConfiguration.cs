using Microsoft.Extensions.Configuration;

namespace YourBrand.Tenancy;

public static class TenantDatabaseConfiguration
{
    public static bool IsDatabasePerTenantEnabled(
        this IConfiguration configuration,
        string? serviceName = null,
        bool defaultValue = false,
        params string[] additionalKeys)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        foreach (var key in GetConfigurationKeys(serviceName).Concat(additionalKeys))
        {
            var value = configuration.GetValue<bool?>(key);

            if (value.HasValue)
            {
                return value.Value;
            }
        }

        return defaultValue;
    }

    private static IEnumerable<string> GetConfigurationKeys(string? serviceName)
    {
        if (!string.IsNullOrWhiteSpace(serviceName))
        {
            yield return $"{serviceName}:Persistence:DatabasePerTenant";
            yield return $"{serviceName}:DatabasePerTenant";
        }

        yield return "Persistence:DatabasePerTenant";
        yield return "DatabasePerTenant";
    }
}
