using System;
using YourBrand.Invoicing.Client;

namespace YourBrand.TimeReport;

public static class ServiceExtensions
{
    public static IServiceCollection AddClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient(nameof(IdentityService.Client.IUsersClient) + "2", (sp, http) =>
        {
            http.BaseAddress = new Uri($"https://localhost:5040/");
            http.DefaultRequestHeaders.Add("X-API-Key", "foobar");
        })
        .AddTypedClient<IdentityService.Client.IUsersClient>((http, sp) => new IdentityService.Client.UsersClient(http));

        services.AddInvoicingClients((sp, http) =>
        {
            http.BaseAddress = new Uri($"https://localhost:5174/api/invoicing/");
        });

        return services;
    }
}

