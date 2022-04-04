using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Shared;

namespace YourBrand.TimeReport;

public static class ServiceExtensions
{
    public static IServiceCollection AddTimeReport(this IServiceCollection services)
    {
        services.AddClients();
        
        return services;
    }

    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddTimeReportClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}timereport/");
        }, builder => {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });

        return services;
    }
}