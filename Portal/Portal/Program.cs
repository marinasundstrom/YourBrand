using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http.Json;

using YourBrand.Portal;
using YourBrand.Portal.Theming;
using Blazored.LocalStorage;

using YourBrand.Portal.Navigation;
using YourBrand.Portal.Modules;
using System.Reflection;
using Humanizer.Localisation;
using Microsoft.Extensions.Localization;
using YourBrand.TimeReport.Client;

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
    .AddThemeServices()
    .AddNavigationServices()
    .AddScoped<ModuleLoader>();

await LoadModules(builder.Services);

var app = builder.Build();

var moduleBuilder = app.Services.GetRequiredService<ModuleLoader>();
moduleBuilder.ConfigureServices();

var navManager = app.Services
    .GetRequiredService<NavManager>();

var resources = app.Services.GetRequiredService<IStringLocalizer<YourBrand.Portal.Resources>>();

var group = navManager.GetGroup("administration") ?? navManager.CreateGroup("administration", () => resources["Administration"]);

group.CreateItem("users", options =>
{
    options.NameFunc = () => resources["Users"];
    options.Icon = MudBlazor.Icons.Material.Filled.Person;
    options.Href = "/users";
    options.RequiresAuthorization = true;
});

group.CreateItem("setup", options =>
{
    options.NameFunc = () => resources["SetUp"];
    options.Icon = MudBlazor.Icons.Material.Filled.Settings;
    options.Href = "/setup";
});

await app.Services.Localize();

await app.RunAsync();

async Task LoadModules(IServiceCollection services)
{
    var http = builder.Services
        .BuildServiceProvider()
        .GetRequiredService<HttpClient>();

    var entries = await http.GetFromJsonAsync<IEnumerable<ModuleDefinition>>($"{ServiceUrls.AppServiceUrl}/Modules");

    entries!.Where(x => x.Enabled).ToList().ForEach(x =>
        ModuleLoader.LoadModule(x.Name, Assembly.Load(x.Assembly), x.Enabled));

    ModuleLoader.AddServices(builder.Services);
}

record ModuleEntry(Assembly Assembly, bool Enabled);

public record ModuleDefinition(string Name, string Assembly, bool Enabled);