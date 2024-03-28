using System;

using YourBrand.Messenger.Client;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using YourBrand.Portal;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Modules;
using Microsoft.Extensions.Localization;

namespace YourBrand.Messenger;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddTransient<CustomAuthorizationMessageHandler>();

        services.AddMessengerClients((sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/messenger/");
        }, builder => builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>());
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.CreateGroup("messenger", () => resources["Messenger"]);
        group.RequiresAuthorization = true;

        group.CreateItem("conversations", () => resources["Conversations"], MudBlazor.Icons.Material.Filled.Chat, "/conversations");
    }
}