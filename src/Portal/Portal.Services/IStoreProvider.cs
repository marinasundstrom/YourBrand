namespace YourBrand.Portal.Services;

public interface IStoreProvider
{
    Task<IEnumerable<Store>> GetAvailableStoresAsync();

    Store? CurrentStore { get; set; }

    Task SetCurrentStore(string storeId);

    event EventHandler? CurrentStoreChanged;
}

public record class Store(string Id, string Name, string Handle, Currency Currency);

public record class Currency(string Code, string Name, string Symbol);