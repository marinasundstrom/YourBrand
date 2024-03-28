using YourBrand.Catalog.Features.Currencies;

namespace YourBrand.Catalog.Features.Stores;

public record class StoreDto(string Id, string Name, string Handle, CurrencyDto Currency);