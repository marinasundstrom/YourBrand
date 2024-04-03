using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;

using YourBrand.Portal;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Widgets;
using YourBrand.Ticketing.Client;

namespace YourBrand.Ticketing;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddTransient<CustomAuthorizationMessageHandler>();

        services.AddTicketingClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.CustomerServiceServiceUrl}/");
        }, builder =>
        {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });

        
        services.AddKeyedScoped<IUserSearchProvider, UserSearchProvider>(ServiceKeys.UserSearchProviderKey);
        //services.AddKeyedScoped<IOrganizationSearchProvider, OrganizationSearchProvider>(ServiceKeys.OrganizationSearchProviderKey);
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var t = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("customer-relations") ?? navManager.CreateGroup("customer-relations", () => t["Customer relations"]);
        group.RequiresAuthorization = true;

        var group2 = group.GetGroup("customer-support") ?? group.CreateGroup("customer-support", () => t["Support"], MudBlazor.Icons.Material.Filled.Support);

        group2.CreateItem("board", () => t["Board"], MudBlazor.Icons.Material.Filled.TableView, "/Tickets/Board");

        group2.CreateItem("tickets", () => t["Tickets"], MudBlazor.Icons.Material.Filled.InsertDriveFile, "/Tickets");

        var widgetService =
            services.GetRequiredService<IWidgetService>();

        widgetService.RegisterWidget(new Widget("tickets.overview", "Tickets", typeof(TicketsWidget))
        {
            Size = WidgetSize.Medium
        });
    }
}

public class UserSearchProvider(IUsersClient usersClient) : IUserSearchProvider
{
    public async Task<IEnumerable<Portal.User>> QueryUsersAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        var result = await usersClient.GetUsersAsync(1, 10, searchTerm, null, null, cancellationToken);
        return result.Items.Select(UserMappings.ToUser);
    }
}

public static class UserMappings 
{
    public static Portal.User ToUser(this User user) => new (user.Id, user.Name);

    public static User ToDto(this Portal.User user) => new () 
    {
        Id = user.Id,
        Name = user.Name
    };
}

/*
public class OrganizationSearchProvider(IOrganizationsClient organizationsClient) : IOrganizationSearchProvider
{
    public Task<IEnumerable<Organization>> QueryOrganizationsAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
*/

public static class ServiceKeys
{
    public readonly static string? UserSearchProviderKey = "TicketingUserSearchProvider";
    public readonly static string? OrganizationSearchProviderKey = "TicketingOrganizationSearchProvider";
}