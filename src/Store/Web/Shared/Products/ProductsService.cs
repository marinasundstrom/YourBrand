namespace BlazorApp.Products;

using System.Collections.Generic;

using YourBrand.StoreFront;

public sealed class ProductsService(IProductsClient productsClient) : IProductsService
{
    public async Task<PagedResult<Product>> GetProducts(string? brandIdOrHandle = null, int? page = 1, int? pageSize = 10, string? searchTerm = null, string? categoryPath = null, CancellationToken cancellationToken = default)
    {
        var results = await productsClient.GetProductsAsync(brandIdOrHandle, page, pageSize, searchTerm, categoryPath, cancellationToken);
        return new PagedResult<Product>(results.Items.Select(product => product.Map()), results.Total);
    }

    public async Task<Product> GetProductById(string productIdOrHandle, CancellationToken cancellationToken = default)
    {
        var product = await productsClient.GetProductByIdAsync(productIdOrHandle, cancellationToken);
        return product.Map();
    }

    public async Task<Product?> FindProductVariantByAttributes(string productIdOrHandle, Dictionary<string, string?> selectedAttributeValues, CancellationToken cancellationToken = default)
    {
        var product = await productsClient.FindProductVariantByAttributesAsync(productIdOrHandle, selectedAttributeValues, cancellationToken);
        return product?.Map()!;
    }

    public async Task<IEnumerable<Product>> FindProductVariantsByAttributes(string productIdOrHandle, Dictionary<string, string?> selectedAttributeValues, CancellationToken cancellationToken = default)
    {
        var products = await productsClient.FindProductVariantsByAttributesAsync(productIdOrHandle, selectedAttributeValues, cancellationToken);
        return products.Select(x => x.Map());
    }

    public async Task<PagedResult<Product>> GetProductVariants(string productIdOrHandle, int page = 1, int pageSize = 10, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        var results = await productsClient.GetProductVariantsAsync(productIdOrHandle, page, pageSize, searchTerm, cancellationToken);
        return new PagedResult<Product>(results.Items.Select(x => x.Map()), results.Total);
    }

    public async Task<IEnumerable<AttributeValue>> GetAvailableProductVariantAttributesValues(string productIdOrHandle, string attributeId, Dictionary<string, string?> selectedAttributeValues, CancellationToken cancellationToken = default)
    {
        var results = await productsClient.GetAvailableVariantAttributeValuesAsync(productIdOrHandle, attributeId, selectedAttributeValues, cancellationToken);
        return results.Select(x => x.Map());
    }
}