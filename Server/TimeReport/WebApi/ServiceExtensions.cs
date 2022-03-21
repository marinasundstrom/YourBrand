using System;

namespace YourCompany.TimeReport;

public static class ServiceExtensions
{
    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddHttpClient(nameof(IdentityService.Client.IUsersClient) + "2", (sp, http) =>
        {
            http.BaseAddress = new Uri($"https://identity.local/");
            http.DefaultRequestHeaders.Add("X-API-KEY", "foobar");
        })
        .AddTypedClient<IdentityService.Client.IUsersClient>((http, sp) => new IdentityService.Client.UsersClient(http));

        return services;
    }
}

