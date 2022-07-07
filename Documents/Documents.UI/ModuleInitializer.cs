using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Shared;
using YourBrand.Documents.Client;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Modules;

namespace YourBrand.Documents;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddDocumentsClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}api/documents/");
        }, builder => {
            //builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var group = navManager.GetGroup("misc") ?? navManager.CreateGroup("misc", "Miscellaneous");
        group.CreateItem("documents", "Documents", MudBlazor.Icons.Material.Filled.InsertDriveFile, "/documents");
    }
}