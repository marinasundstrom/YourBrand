using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using MudBlazor;

using YourBrand.Portal.AppBar;
using YourBrand.Portal.Authentication;
using YourBrand.Portal.Authentication;

namespace YourBrand.Portal.Authentication;

public static class ServiceProviderExtensions
{
    public static IServiceProvider UseAuthentication(this IServiceProvider services)
    {
        AddAppBarTrayItems(services);

        return services;
    }

    private static void AddAppBarTrayItems(IServiceProvider services)
    {
        var appBarTray = services
            .GetRequiredService<IAppBarTrayService>();

        var t = services.GetRequiredService<IStringLocalizer<AppBar.AppBar>>();

        var loginDisplayId = "Shell.LoginDisplay";

        appBarTray.AddItem(new AppBarTrayItem(loginDisplayId, string.Empty, typeof(LoginDisplay)));
    }
}