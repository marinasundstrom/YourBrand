using Meziantou.Extensions.Logging.Xunit;

using Microsoft.AspNetCore.Mvc.Testing;

using Xunit.Abstractions;

namespace YourBrand.StoreFront.IntegrationTests;

public static class WebApplicationFactoryExtensions
{
    public static WebApplicationFactory<TStartup> WithTestLogging<TStartup>(this WebApplicationFactory<TStartup> factory, ITestOutputHelper testOutputHelper) where TStartup : class
    {
        return factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureLogging(logging =>
            {
                // check if scopes are used in normal operation
                //var useScopes = logging.UsesScopes();

                // remove other logging providers, such as remote loggers or unnecessary event logs
                logging.ClearProviders();
                logging.Services.AddSingleton<ILoggerProvider>(r => new XUnitLoggerProvider(testOutputHelper /*, useScopes */));
            });
        });
    }
}