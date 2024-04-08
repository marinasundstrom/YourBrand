using Blazored.LocalStorage;

using YourBrand.IdentityManagement.Client;
using YourBrand.Portal.Services;

namespace YourBrand.Portal;

public sealed class OrganizationProvider(IOrganizationsClient storesClient, ILocalStorageService localStorageService) : IOrganizationProvider
{
    IEnumerable<Portal.Services.Organization> _organizations;

    public async Task<IEnumerable<Portal.Services.Organization>> GetAvailableOrganizationsAsync()
    {
        var items = _organizations = (await storesClient.GetOrganizationsAsync(0, null, null, null, null)).Items
            .Select(x => new Portal.Services.Organization(x.Id, x.Name, x.FriendlyName));

        if (CurrentOrganization is null)
        {
            var storeId = await localStorageService.GetItemAsStringAsync("organizationId");
            await SetCurrentOrganization(storeId ?? items.First().Id);
        }
        return items;
    }

    public Portal.Services.Organization? CurrentOrganization { get; set; }

    public async Task SetCurrentOrganization(string storeId)
    {
        if (_organizations is null)
        {
            await GetAvailableOrganizationsAsync();
        }

        CurrentOrganization = _organizations!.FirstOrDefault(x => x.Id == storeId);

        await localStorageService.SetItemAsStringAsync("organizationId", storeId);

        CurrentOrganizationChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? CurrentOrganizationChanged;
}