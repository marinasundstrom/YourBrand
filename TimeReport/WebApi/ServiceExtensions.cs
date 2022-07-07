using System;
using YourBrand.Invoicing.Client;

namespace YourBrand.TimeReport;

public static class ServiceExtensions
{
    public static IServiceCollection AddClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient(nameof(IdentityService.Client.IUsersClient) + "2", (sp, http) =>
        {
            http.BaseAddress = new Uri($"https://identity.local/");
            http.DefaultRequestHeaders.Add("X-API-KEY", "foobar");
        })
        .AddTypedClient<IdentityService.Client.IUsersClient>((http, sp) => new IdentityService.Client.UsersClient(http));

        services.AddInvoicingClients((sp, http) =>
        {
            http.BaseAddress = new Uri($"{configuration.GetServiceUri("nginx", "https")}/api/invoicing/");
        });

        return services;
    }
}

