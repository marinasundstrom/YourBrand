using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Portal;
using YourBrand.Portal.Modules;
using YourBrand.Sales.OrderManagement;
using YourBrand.Sales.Subscriptions;

namespace YourBrand.Sales;

public class ModuleInitializer : IModuleInitializer
{
    public static void Initialize(IServiceCollection services)
    {
        services.AddTransient<CustomAuthorizationMessageHandler>();

        services.AddSalesClients((sp, httpClient) =>
        {
            var navigationManager = sp.GetRequiredService<NavigationManager>();
            httpClient.BaseAddress = new Uri($"{ServiceUrls.OrdersServiceUrl}/");
        }, builder =>
        {
            builder.AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
        });

        services.AddKeyedScoped<IUserSearchProvider, UserSearchProvider>(ServiceKeys.UserSearchProviderKey);
        services.AddKeyedScoped<IOrganizationSearchProvider, OrganizationSearchProvider>(ServiceKeys.OrganizationSearchProviderKey);
    }

    public static void ConfigureServices(IServiceProvider services)
    {
        services.UseOrderManagement();
        services.UseSubscriptions();
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
    public static Portal.User ToUser(this YourBrand.Sales.User user) => new(user.Id, user.Name);

    public static YourBrand.Sales.User ToDto(this Portal.User user) => new()
    {
        Id = user.Id,
        Name = user.Name
    };

    public static Portal.Organization ToOrganization(this YourBrand.Sales.Organization organization) => new(organization.Id, organization.Name);

    public static YourBrand.Sales.Organization ToDto(this Portal.Organization organization) => new()
    {
        Id = organization.Id,
        Name = organization.Name
    };
}

public class OrganizationSearchProvider(IOrganizationsClient organizationsClient) : IOrganizationSearchProvider
{
    public async Task<IEnumerable<Portal.Organization>> QueryOrganizationsAsync(string? searchTerm, CancellationToken cancellationToken)
    {
        var result = await organizationsClient.GetOrganizationsAsync(1, 10, searchTerm, null, null, cancellationToken);
        return result.Items.Select(UserMappings.ToOrganization);
    }
}

public static class ServiceKeys
{
    public readonly static string? UserSearchProviderKey = "SalesUserSearchProvider";
    public readonly static string? OrganizationSearchProviderKey = "SalesOrganizationSearchProvider";
}