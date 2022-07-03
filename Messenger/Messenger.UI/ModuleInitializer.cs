using System;

using YourBrand.Messenger.Client;

using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using YourBrand.Portal.Shared;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Modules;

namespace YourBrand.Messenger;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
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

        var group = navManager.CreateGroup("messenger", "Messenger");
        group.CreateItem("conversations", "Conversations", MudBlazor.Icons.Material.Filled.Chat, "/conversations");
    }
}