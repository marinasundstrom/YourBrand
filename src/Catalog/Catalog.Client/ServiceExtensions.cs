namespace YourBrand.Catalog;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions
{
    public static IServiceCollection AddCatalogClients(this IServiceCollection services, Uri baseUrl, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        services.AddCatalogClients((sp, http) =>
        {
            http.BaseAddress = baseUrl;
        }, configureBuilder);

        return services;
    }

    public static IServiceCollection AddCatalogClients(this IServiceCollection services, Action<IServiceProvider, HttpClient> configureClient, Action<IHttpClientBuilder>? configureBuilder = null)
    {
        IHttpClientBuilder builder = services.AddHttpClient("CatalogAPI", configureClient);

        configureBuilder?.Invoke(builder);

        services.AddHttpClient<IProductsClient>("CatalogAPI")
            .AddTypedClient<IProductsClient>((http, sp) => new YourBrand.Catalog.ProductsClient(http));

        services.AddHttpClient<IProductCategoriesClient>("CatalogAPI")
            .AddTypedClient<IProductCategoriesClient>((http, sp) => new YourBrand.Catalog.ProductCategoriesClient(http));

        services.AddHttpClient<IAttributesClient>("CatalogAPI")
            .AddTypedClient<IAttributesClient>((http, sp) => new YourBrand.Catalog.AttributesClient(http));

        services.AddHttpClient<IProductOptionsClient>("CatalogAPI")
            .AddTypedClient<IProductOptionsClient>((http, sp) => new YourBrand.Catalog.ProductOptionsClient(http));

        services.AddHttpClient<IOptionsClient>("CatalogAPI")
            .AddTypedClient<IOptionsClient>((http, sp) => new YourBrand.Catalog.OptionsClient(http));

        services.AddHttpClient<IAttributesClient>("CatalogAPI")
            .AddTypedClient<IAttributesClient>((http, sp) => new YourBrand.Catalog.AttributesClient(http));

        services.AddHttpClient<IBrandsClient>("CatalogAPI")
            .AddTypedClient<IBrandsClient>((http, sp) => new YourBrand.Catalog.BrandsClient(http));

        services.AddHttpClient<IStoresClient>("CatalogAPI")
            .AddTypedClient<IStoresClient>((http, sp) => new YourBrand.Catalog.StoresClient(http));

        services.AddHttpClient<ICurrenciesClient>("CatalogAPI")
            .AddTypedClient<ICurrenciesClient>((http, sp) => new YourBrand.Catalog.CurrenciesClient(http));

        services.AddHttpClient<IVatRatesClient>("CatalogAPI")
            .AddTypedClient<IVatRatesClient>((http, sp) => new YourBrand.Catalog.VatRatesClient(http));

        return services;
    }
}