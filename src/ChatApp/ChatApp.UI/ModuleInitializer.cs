using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using YourBrand.Portal;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using YourBrand.ChatApp.Markdown;

using YourBrand.ChatApp.Client;
using MudBlazor;
using YourBrand.ChatApp.Chat.Messages;

namespace YourBrand.ChatApp;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddMarkdownServices();

        services.AddSingleton<ITimeViewService, TimeViewService>();

        services.AddTransient<CustomAuthorizationMessageHandler>();

        services.AddChatAppClients((sp, http) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            http.BaseAddress = new Uri($"{navigationManager.BaseUri}api/messenger/");
        }, builder => builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>());

        services.AddEmojiService((sp, http) =>
        {
           var navigationManager = sp.GetRequiredService<NavigationManager>();
           http.BaseAddress = new Uri($"{navigationManager.BaseUri}/");
        }, builder => builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>());
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.CreateGroup("messenger", () => resources["Messenger"]);
        group.RequiresAuthorization = true;

        group.CreateItem("new-channel", () => resources["New channel"], MudBlazor.Icons.Material.Filled.Add, async () => await CreateChannel(services));
        //group.CreateItem("channels", () => resources["Channels"], MudBlazor.Icons.Material.Filled.Chat, "/channels");
    }

    static async Task CreateChannel(IServiceProvider services)
    {
        var t = services.GetRequiredService<IStringLocalizer<Resources>>();
        var dialogService = services.GetRequiredService<IDialogService>();
        var navigationManager = services.GetRequiredService<NavigationManager>();

        var dialogRef = await dialogService.ShowAsync<Chat.Channels.NewChannelDialog>(t["NewChannel"]);
        var result = await dialogRef.Result;

        if (result.Canceled) return;

        var channel = (ChatApp.Channel)result.Data;

        //channels.Add(channel);

        navigationManager.NavigateTo($"/channels/{channel.Id}");
    }
}