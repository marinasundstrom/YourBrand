using System.Globalization;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;

using MudBlazor;

using YourBrand.Portal.AppBar;
using YourBrand.Portal.Localization;

namespace YourBrand.Portal.Localization;

public static class ServiceProviderExtensions
{
    public static IServiceProvider UseLocalization(this IServiceProvider services)
    {
        AddAppBarTrayItems(services);

        return services;
    }

    private static void AddAppBarTrayItems(IServiceProvider services)
    {
        var appBarTray = services
            .GetRequiredService<IAppBarTrayService>();

        var t = services.GetRequiredService<IStringLocalizer<AppBar.AppBar>>();

        var localeSelector = "Shell.LocaleSelector";

        appBarTray.AddItem(new AppBarTrayItem(localeSelector, () => t["Locale"], MudBlazor.Icons.Material.Filled.Language, async () =>
        {
            var dialogService = services.GetRequiredService<IDialogService>();
            var dialogRef = dialogService.Show<CultureSelector>(t["ChangeLocale"]);
            await dialogRef.Result;
        }));
    }

    public static async Task ApplyLocalization(this IServiceProvider serviceProvider)
    {
        CultureInfo culture;
        var js = serviceProvider.GetRequiredService<IJSRuntime>();
        var result = await js.InvokeAsync<string>("blazorCulture.get");

        if (result != null)
        {
            culture = new CultureInfo(result);
        }
        else
        {
            culture = new CultureInfo("en-US");
            await js.InvokeVoidAsync("blazorCulture.set", "en-US");
        }

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}