using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Localization;

using YourBrand.Portal;
using YourBrand.Portal.Modules;
using YourBrand.Portal.Navigation;
using YourBrand.TimeReport.Client;

namespace YourBrand.TimeReport;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.TryAddTransient<CustomAuthorizationMessageHandler>();

        services.AddTimeReportClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.TimeReportServiceUrl}/");
        }, builder =>
        {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });

        services.AddKeyedScoped<IUserSearchProvider, UserSearchProvider>(ServiceKeys.UserSearchProviderKey);
        services.AddKeyedScoped<IOrganizationSearchProvider, OrganizationSearchProvider>(ServiceKeys.OrganizationSearchProviderKey);
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        var navManager = services
            .GetRequiredService<NavManager>();

        var resources = services.GetRequiredService<IStringLocalizer<Resources>>();

        var group = navManager.CreateGroup("project-management", () => resources["Project management"]);
        group.RequiresAuthorization = true;

        group.CreateItem("projects", () => resources["Projects"], MudBlazor.Icons.Material.Filled.List, "/projects");
        group.CreateItem("report-time", () => resources["Report time"], MudBlazor.Icons.Material.Filled.AccessTime, "/timesheet");
        group.CreateItem("timesheets", options =>
        {
            options.SetName(() => resources["Timesheets"]);
            options.Icon = MudBlazor.Icons.Material.Filled.ListAlt;
            options.Href = "/timesheets";
            options.RequiresAuthorization = true;
            options.Roles = ["Administrator"];
        });
        group.CreateItem("generate-reports", () => resources["GenerateReport"], MudBlazor.Icons.Material.Filled.ListAlt, "/reports");
    }
}

public class UserSearchProvider(IUsersClient usersClient) : IUserSearchProvider
{
    public async Task<IEnumerable<Portal.User>> QueryUsersAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        var result = await usersClient.GetUsersAsync(0, 10, searchTerm, null, null, cancellationToken);
        return result.Items.Select(UserMappings.ToUser);
    }
}

public static class UserMappings
{
    public static Portal.User ToUser(this Client.User user) => new(user.Id, user.DisplayName ?? $"{user.FirstName} {user.LastName}");

    public static Client.User ToDto(this Portal.User user) => new()
    {
        Id = user.Id,
        DisplayName = user.Name
    };

    public static Portal.Organization ToOrganization(this Client.Organization organization) => new(organization.Id, organization.Name);

    public static Client.Organization ToDto(this Portal.Organization organization) => new()
    {
        Id = organization.Id,
        Name = organization.Name
    };
}

public class OrganizationSearchProvider(IOrganizationsClient organizationsClient) : IOrganizationSearchProvider
{
    public async Task<IEnumerable<Portal.Organization>> QueryOrganizationsAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        var result = await organizationsClient.GetOrganizationsAsync(0, 10, searchTerm, null, null, cancellationToken);
        return result.Items.Select(UserMappings.ToOrganization);
    }
}

public static class ServiceKeys
{
    public readonly static string? UserSearchProviderKey = "TimeReportUserSearchProvider";
    public readonly static string? OrganizationSearchProviderKey = "TimeReportOrganizationSearchProvider";
}