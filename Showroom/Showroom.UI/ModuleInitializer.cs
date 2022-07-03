using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Shared;
using YourBrand.Showroom.Client;

namespace YourBrand.Showroom;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddShowroomClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}api/showroom/");
        }, builder => {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var group = navManager.CreateGroup("showroom", "Showroom");
        group.CreateItem("consultants", "Consultants", MudBlazor.Icons.Material.Filled.Person, "/consultants");
    }
}