using System.Net.Http.Json;
using System.Reflection;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using MudBlazor;
using MudBlazor.Utilities;

using YourBrand.AppService.Client;
using YourBrand.Portal;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Theming;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Local", options.ProviderOptions);

    options.UserOptions.NameClaim = "name";
    options.UserOptions.RoleClaim = "role";
});

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorage();

builder.Services
    .AddServices()
    .AddShellServices()
    .AddScoped<ModuleLoader>();

await LoadModules(builder.Services);

var app = builder.Build();

var moduleBuilder = app.Services.GetRequiredService<ModuleLoader>();
moduleBuilder.ConfigureServices();

app.Services.UseShell();

await app.Services.Localize();

await LoadBrandProfile(builder.Services);

await app.RunAsync();

async Task LoadModules(IServiceCollection services)
{
    var modulesClient = builder.Services
        .BuildServiceProvider()
        .GetRequiredService<IModulesClient>();

    var moduleEntries = await modulesClient.GetModulesAsync();

    moduleEntries!.Where(x => x.Enabled).ToList().ForEach(x =>
        ModuleLoader.LoadModule(x.Name, Assembly.Load(x.Assembly), x.Enabled));

    ModuleLoader.AddServices(builder.Services);
}

async Task LoadBrandProfile(IServiceCollection services)
{
    var themeManager = builder.Services
        .BuildServiceProvider()
        .GetRequiredService<IThemeManager>();

    var brandProfileClient = builder.Services
        .BuildServiceProvider()
        .GetRequiredService<IBrandProfileClient>();

    themeManager.SetTheme(Themes.AppTheme);

    try 
    {
        var brandProfile = await brandProfileClient.GetBrandProfileAsync();

        if (brandProfile is not null)
        {
            var theme = new MudTheme();

            if(brandProfile.Colors.Light is BrandColorPalette colorPalette) 
            {
                if (colorPalette.BackgroundColor is not null) 
                {
                    theme.Palette.Background = colorPalette.BackgroundColor;
                }
                if (colorPalette.AppbarBackgroundColor is not null)
                {
                    theme.Palette.AppbarBackground = colorPalette.AppbarBackgroundColor;
                }
                if (colorPalette.PrimaryColor is not null)
                {
                    theme.Palette.Primary = colorPalette.PrimaryColor;
                }
                if (colorPalette.SecondaryColor is not null)
                {
                    theme.Palette.Secondary = colorPalette.SecondaryColor;
                }
            }

            if (brandProfile.Colors.Dark is BrandColorPalette colorPalette2)
            {
                if (colorPalette2.BackgroundColor is not null)
                {
                    theme.PaletteDark.Background = colorPalette2.BackgroundColor;
                }
                if (colorPalette2.AppbarBackgroundColor is not null)
                {
                    theme.PaletteDark.AppbarBackground = colorPalette2.AppbarBackgroundColor;
                }
                if (colorPalette2.PrimaryColor is not null)
                {
                    theme.PaletteDark.Primary = colorPalette2.PrimaryColor;
                }
                if (colorPalette2.SecondaryColor is not null)
                {
                    theme.PaletteDark.Secondary = colorPalette2.SecondaryColor;
                }
            }

            themeManager.SetTheme(theme);
        }
    }
    catch(Exception) {}
}

sealed record ModuleEntry(Assembly Assembly, bool Enabled);

public record ModuleDefinition(string Name, string Assembly, bool Enabled);