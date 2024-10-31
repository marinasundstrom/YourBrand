using YourBrand.Catalog.Features.Brands;
using YourBrand.Catalog.Features.Currencies;
using YourBrand.Catalog.Features.Producers;
using YourBrand.Catalog.Features.ProductManagement;
using YourBrand.Catalog.Features.Stores;
using YourBrand.Catalog.Features.VatRates;

namespace YourBrand.Catalog.Features;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapFeaturesEndpoints(this IEndpointRouteBuilder app)
    {
        app
        .MapBrandsEndpoints()
        .MapProducersEndpoints()
        .MapCurrenciesEndpoints()
        .MapVatRatesEndpoints()
        .MapProductManagementEndpoints()
        .MapStoresEndpoints();

        return app;
    }
}