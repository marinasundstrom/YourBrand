using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using YourBrand.Portal;
using YourBrand.Showroom.Client;

namespace YourBrand.Showroom;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddTransient<CustomAuthorizationMessageHandler>();

        services.AddShowroomClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.ShowroomServiceUrl}/");
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

        var group2 = navManager.GetGroup("sales") ?? navManager.CreateGroup("sales", () => resources["Sales"]);
        group2.RequiresAuthorization = true;

        var group = group2.CreateGroup("showroom", () => resources["Consultants"], MudBlazor.Icons.Material.Filled.People);
        
        group.CreateItem("search", () => resources["Search"], MudBlazor.Icons.Material.Filled.Search, "/profiles/find");
        group.CreateItem("profiles", () => resources["Profiles"], MudBlazor.Icons.Material.Filled.Person, "/profiles");
        group.CreateItem("cases", () => resources["Cases"], MudBlazor.Icons.Material.Filled.Task, "/cases");
        group.CreateItem("skills", () => resources["Skills"], MudBlazor.Icons.Material.Filled.List, "/skills");
        group.CreateItem("competenceAreas", () => resources["CompetenceAreas"], MudBlazor.Icons.Material.Filled.List, "/competenceareas");
        group.CreateItem("companies", () => resources["Companies"], MudBlazor.Icons.Material.Filled.List, "/companies");
        group.CreateItem("industries", () => resources["Industries"], MudBlazor.Icons.Material.Filled.List, "/industries");
    }
}