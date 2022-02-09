using System;

using Catalog.Application;
using Catalog.Application.Common.Interfaces;
using Catalog.Infrastructure;

using Catalog.WebApi.Hubs;
using Catalog.WebApi.Services;

namespace Catalog.WebApi;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ICurrentUserService, CurrentUserService>();
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

        return services;
    }
}