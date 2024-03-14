using System;

using YourBrand.Identity;
using YourBrand.Showroom.Application;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Infrastructure;

using YourBrand.Showroom.WebApi.Services;

namespace YourBrand.Showroom.WebApi;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddIdentityServices();
        services.AddScoped<IUrlHelper, UrlHelper>();

        services.AddClients();

        services.AddScoped<IFileUploaderService, FileUploaderService>();

        return services;
    }

    private static IServiceCollection AddClients(this IServiceCollection services)
    {
        /*
        services.AddScoped<IItemsClient, ItemsClient>();
        services.AddScoped<IWorkerClient, WorkerClient>();
        services.AddScoped<INotificationClient, NotificationClient>();
        services.AddScoped<ISomethingClient, SomethingClient>();

        services.AddHttpClient(nameof(IdentityService.Client.IUsersClient) + "2", (sp, http) =>
        {
            http.BaseAddress = new Uri($"https://localhost:5040/");
            http.DefaultRequestHeaders.Add("X-API-Key", "foobar");
        })
        .AddTypedClient<IdentityService.Client.IUsersClient>((http, sp) => new IdentityService.Client.UsersClient(http));
        */

        return services;
    }
}