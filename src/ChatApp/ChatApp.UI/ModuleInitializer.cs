using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

using MudBlazor;

using YourBrand.ChatApp.Chat.Messages;
using YourBrand.ChatApp.Client;
using YourBrand.ChatApp.Markdown;
using YourBrand.Portal;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;

namespace YourBrand.ChatApp;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddMarkdownServices();

        services.AddSingleton<ITimeViewService, TimeViewService>();

        services.TryAddTransient<CustomAuthorizationMessageHandler>();

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

    public static Task ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.CreateGroup("messenger", options =>
        {
            options.NameFunc = () => resources["Messenger"];
            options.Component = typeof(ChatMenuItem);
        });

        group.RequiresAuthorization = true;

        //group.CreateItem("new-channel", () => resources["New channel"], MudBlazor.Icons.Material.Filled.Add, async () => await CreateChannel(services));
        //group.CreateItem("channels", () => resources["Channels"], MudBlazor.Icons.Material.Filled.Chat, "/channels");

        return Task.CompletedTask;
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