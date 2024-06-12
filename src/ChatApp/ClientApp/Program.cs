using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.FeatureManagement;
using MudBlazor.Services;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using YourBrand.ChatApp;
using YourBrand.ChatApp.Theming;
using YourBrand.ChatApp.Chat.Messages;
using MudBlazor;
using YourBrand.ChatApp.Markdown;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddFeatureManagement();

builder.Services.AddTransient<CustomAuthorizationMessageHandler>();

builder.Services.AddMarkdownServices();

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Local", options.ProviderOptions);
});

builder.Services.AddScoped<ChatApp.Services.IAccessTokenProvider, ChatApp.Services.AccessTokenProvider>();

builder.Services.AddScoped<ChatApp.Services.IUserContext, ChatApp.Services.UserContext>();

builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomLeft;

    config.SnackbarConfiguration.PreventDuplicates = false;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.ShowCloseIcon = true;
    config.SnackbarConfiguration.VisibleStateDuration = 10000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 500;
    config.SnackbarConfiguration.SnackbarVariant = Variant.Filled;
});

//builder.Services.AddScoped<MudEmojiPicker.Data.EmojiService>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddThemeServices();

builder.Services.AddLocalization();

builder.Services.AddSingleton<ITimeViewService, TimeViewService>();

var app = builder.Build();

await app.Services.Localize();

await app.RunAsync();

IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
         .HandleTransientHttpError()
         .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5));
}