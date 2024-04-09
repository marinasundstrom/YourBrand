using System.Net.Http.Json;
using System.Reflection;

using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

using YourBrand.Portal;
using YourBrand.Portal.Modules;

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


await app.RunAsync();

async Task LoadModules(IServiceCollection services)
{
    var http = builder.Services
        .BuildServiceProvider()
        .GetRequiredService<HttpClient>();

    var entries = await http.GetFromJsonAsync<IEnumerable<ModuleDefinition>>($"{ServiceUrls.AppServiceUrl}/v1/Modules");

    entries!.Where(x => x.Enabled).ToList().ForEach(x =>
        ModuleLoader.LoadModule(x.Name, Assembly.Load(x.Assembly), x.Enabled));

    ModuleLoader.AddServices(builder.Services);
}

sealed record ModuleEntry(Assembly Assembly, bool Enabled);

public record ModuleDefinition(string Name, string Assembly, bool Enabled);