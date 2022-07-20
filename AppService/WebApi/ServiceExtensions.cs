using System;

using YourBrand.Application;
using YourBrand.Application.Common.Interfaces;
using YourBrand.Identity;
using YourBrand.Infrastructure;

using YourBrand.WebApi.Hubs;
using YourBrand.WebApi.Services;

namespace YourBrand.WebApi;

public static class ServiceExtensions
{
    public  static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddIdentityServices();
        services.AddScoped<IUrlHelper, UrlHelper>();

        services.AddClients();

        services.AddScoped<IFileUploaderService, FileUploaderService>();

        return services;
    }

    private static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddScoped<IItemsClient, ItemsClient>();
        services.AddScoped<IWorkerClient, WorkerClient>();
        services.AddScoped<INotificationClient, NotificationClient>();
        services.AddScoped<ISomethingClient, SomethingClient>();

        services.AddHttpClient(nameof(IdentityService.Client.IUsersClient) + "2", (sp, http) =>
        {
            http.BaseAddress = new Uri($"https://identity.local/");
            http.DefaultRequestHeaders.Add("X-API-KEY", "foobar");
        })
        .AddTypedClient<IdentityService.Client.IUsersClient>((http, sp) => new IdentityService.Client.UsersClient(http));

        return services;
    }
}