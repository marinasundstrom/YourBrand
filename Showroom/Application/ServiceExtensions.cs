using System;
using System.Net.Http.Headers;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.PersonProfiles.Queries;

using MediatR;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YourBrand.Notifications.Client;
using YourBrand.Identity;

namespace YourBrand.Showroom.Application;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(typeof(GetPersonProfilesAsync));
        
        services.AddScoped<Handler>();

        services.AddNotificationsClients((sp, http) =>
        {
            http.BaseAddress = configuration.GetServiceUri("notifications");
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