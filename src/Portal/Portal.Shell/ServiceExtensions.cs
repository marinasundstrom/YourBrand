using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.AppBar;
using YourBrand.Portal.Markdown;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Services;
using YourBrand.Portal.Theming;
using YourBrand.Portal.Widgets;

namespace YourBrand.Portal;

public static class ServiceExtensions
{
    public static IServiceCollection AddShellServices(this IServiceCollection services)
    {
        services
            .AddWidgets()
            .AddThemeServices()
            .AddNavigationServices()
            .AddAppBar()
            .AddTransient<CustomAuthorizationMessageHandler>()
            .AddScoped<IUserContext, UserContext>()
            .AddScoped<IAccessTokenProvider, AccessTokenProvider>()
            .AddMarkdownServices();

        return services;
    }
}