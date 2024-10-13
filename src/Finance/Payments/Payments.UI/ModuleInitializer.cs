using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

using YourBrand.Payments.Client;
using YourBrand.Portal;
using YourBrand.Portal.Modules;

namespace YourBrand.Payments;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.TryAddTransient<CustomAuthorizationMessageHandler>();

        services.AddPaymentsClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.PaymentsServiceUrl}/");
        }, builder =>
        {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        /*
        var navManager = services
            .GetRequiredService<NavManager>();

        var group = navManager.GetGroup("finance") ?? navManager.CreateGroup("finance", "Finance");
        group.CreateItem("payments", "Payments", MudBlazor.Icons.Material.Filled.List, "/payments");
        */
    }
}