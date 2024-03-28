using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal;
using YourBrand.IdentityManagement.Client;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Modules;
using Microsoft.Extensions.Localization;

namespace YourBrand.IdentityManagement;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddTransient<CustomAuthorizationMessageHandler>();

        services.AddIdentityManagementClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.IdentityManagementServiceUrl}/");
        }, builder =>
        {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.CreateGroup("administration", () => resources["Administration"]) ?? navManager.CreateGroup("administration", () => resources["Administration"]);
        group.RequiresAuthorization = true;
        //group.Roles = new[] { "Foo" } ;

        group.CreateItem("users", () => resources["Users"], MudBlazor.Icons.Material.Filled.People, "/usermanagement/users");
        group.CreateItem("organization", () => resources["Organization"], MudBlazor.Icons.Material.Filled.House, "/usermanagement/organization");
    }
}