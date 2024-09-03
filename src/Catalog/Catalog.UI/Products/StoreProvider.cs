using Blazored.LocalStorage;

using YourBrand.Portal.Services;

namespace YourBrand.Catalog;

public sealed class StoreProvider(IStoresClient storesClient, IOrganizationProvider organizationProvider, ILocalStorageService localStorageService) : IStoreProvider
{
    IEnumerable<Portal.Services.Store> _stores;

    public async Task<IEnumerable<Portal.Services.Store>> GetAvailableStoresAsync()
    {
        var organization = await organizationProvider.GetCurrentOrganizationAsync();

        var items = _stores = (await storesClient.GetStoresAsync(organization.Id, 0, null, null, null, null)).Items
            .Select(x => new Portal.Services.Store(x.Id, x.Name, x.Handle, new Portal.Services.Currency(x.Currency.Code, x.Currency.Name, x.Currency.Symbol)));

        if (CurrentStore is null)
        {
            var storeId = await localStorageService.GetItemAsStringAsync("storeId");
            await SetCurrentStore(storeId ?? items.First().Id);
        }
        return items;
    }

    public Portal.Services.Store? CurrentStore { get; set; }

    public async Task SetCurrentStore(string storeId)
    {
        if (_stores is null)
        {
            await GetAvailableStoresAsync();
        }

        CurrentStore = _stores!.FirstOrDefault(x => x.Id == storeId);

        await localStorageService.SetItemAsStringAsync("storeId", storeId);

        CurrentStoreChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler? CurrentStoreChanged;
}