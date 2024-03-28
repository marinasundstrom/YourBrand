using System;
using System.Net.Http.Headers;

using YourBrand.Application.Common.Interfaces;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YourBrand.Notifications.Client;
using YourBrand.Identity;

namespace YourBrand.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(ServiceExtensions)));

        services.AddScoped<Handler>();

        services.AddNotificationsClients((sp, http) =>
        {
            http.BaseAddress = new Uri("https://localhost:5174/api/notifications/");
        },
        builder =>
        {
            builder.AddHttpMessageHandler<Handler>();
        });

        return services;
    }
}

public class Handler : DelegatingHandler
{
    private readonly ICurrentUserService _currentUserService;

    public Handler(ICurrentUserService currentUserService)
    {
        _currentUserService = currentUserService;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        request.Headers.Authorization = new AuthenticationHeaderValue("bearer", _currentUserService.GetAccessToken());

        return base.SendAsync(request, cancellationToken);
    }
}