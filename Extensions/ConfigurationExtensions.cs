using Azure.Identity;

using Microsoft.Extensions.Configuration;

namespace YourBrand.Extensions;

public static class ConfigurationExtensions
{
    public static IConfigurationBuilder AddAzureAppConfiguration(this ConfigurationManager configurationManager, IConfiguration configuration)
    {
        configurationManager.AddAzureAppConfiguration(options =>
            options.Connect(
                new Uri($"https://{configuration["Azure:AppConfig:Name"]}.azconfig.io"),
                new DefaultAzureCredential()));

        return configurationManager;
    }

    public static IConfigurationBuilder AddAzureKeyVault(this ConfigurationManager configurationManager, IConfiguration configuration)
    {
        configurationManager.AddAzureKeyVault(
            new Uri($"https://{configuration["Azure:KeyVault:Name"]}.vault.azure.net/"),
            new DefaultAzureCredential());

        return configurationManager;
    }
}
