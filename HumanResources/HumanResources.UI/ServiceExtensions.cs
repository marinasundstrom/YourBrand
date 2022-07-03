using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal.Shared;
using YourBrand.HumanResources.Client;
using YourBrand.Portal.Navigation;

namespace YourBrand.HumanResources;

public static class ServiceExtensions
{
    public static IServiceCollection AddHumanResources(this IServiceCollection services)
    {
        services.AddClients();
        
        return services;
    }

    public static IServiceCollection AddClients(this IServiceCollection services)
    {
        services.AddHumanResourcesClients((sp, httpClient) => {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{navigationManager.BaseUri}api/humanresources/");
        }, builder => {
            //builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });

        return services;
    }

    public static IServiceProvider UseHumanResources(this IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var group = navManager.AddGroup("human-resources", "Human Resources");
        group.AddItem("persons", "Persons", MudBlazor.Icons.Material.Filled.Person, "/users");
        group.AddItem("teams", "Teams", MudBlazor.Icons.Material.Filled.People, "/teams");
        
        return services;
    }
}