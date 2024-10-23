using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

using YourBrand.Meetings.Client;
using YourBrand.Portal;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using YourBrand.Portal.Widgets;

namespace YourBrand.Meetings;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.TryAddTransient<CustomAuthorizationMessageHandler>();
        services.TryAddTransient<ErrorHandler>();

        services.AddMeetingsClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.MeetingsServiceUrl}/");
        }, builder =>
        {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
            builder.AddHttpMessageHandler<ErrorHandler>();
        });

        services.AddKeyedScoped<IUserSearchProvider, UserSearchProvider>(ServiceKeys.UserSearchProviderKey);
        //services.AddKeyedScoped<IOrganizationSearchProvider, OrganizationSearchProvider>(ServiceKeys.OrganizationSearchProviderKey);
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var t = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.GetGroup("executive") ?? navManager.CreateGroup("executive", () => t["Executive"]);
        group.RequiresAuthorization = true;

        var group2 = group.GetGroup("meetings") ?? group.CreateGroup("meetings", () => t["Meetings"], MudBlazor.Icons.Material.Filled.MeetingRoom);

        group2.CreateItem("meetings", () => t["List"], MudBlazor.Icons.Material.Filled.List, "/Meetings");

        group2.CreateItem("motions", () => t["Motions"], MudBlazor.Icons.Material.Filled.FilePresent, "/Meetings/Motions");

        group2.CreateItem("minutes", () => t["Minutes"], MudBlazor.Icons.Material.Filled.FileOpen, "/Meetings/Minutes");

        group2.CreateItem("groups", () => t["Groups"], MudBlazor.Icons.Material.Filled.Group, "/Meetings/Groups");

        /*
        group2.CreateItem("upcoming", () => t["Upcoming"], MudBlazor.Icons.Material.Filled.List, "/Meetings/Upcoming");

        group2.CreateItem("archive", () => t["Archive"], MudBlazor.Icons.Material.Filled.Archive, "/Meetings/Archive"); */

        /*
        group2.CreateItem("tickets", () => t["Tickets"], MudBlazor.Icons.Material.Filled.InsertDriveFile, "/Tickets");

        var widgetService =
            services.GetRequiredService<IWidgetService>();

        widgetService.RegisterWidget(new Widget("tickets.overview", "Tickets", typeof(TicketsWidget))
        {
            Size = WidgetSize.Medium
        });
        */
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
    public static Portal.User ToUser(this User user) => new(user.Id, user.Name);

    public static User ToDto(this Portal.User user) => new()
    {
        Id = user.Id,
        Name = user.Name
    };
}

public static class ServiceKeys
{
    public readonly static string? UserSearchProviderKey = "MeetingsUserSearchProvider";
    public readonly static string? OrganizationSearchProviderKey = "MeetingsOrganizationSearchProvider";
}