
namespace BlazorApp.Products;

public interface IProductsService
{
    Task<PagedResult<Product>> GetProducts(string? brandIdOrHandle = null, int? page = 1, int? pageSize = 10, string? searchTerm = null, string? categoryPath = null, CancellationToken cancellationToken = default);

    Task<Product> GetProductById(string productIdOrHandle, CancellationToken cancellationToken = default);

    Task<ProductPriceResult> CalculatePrice(string productIdOrHandle, CalculateProductPriceRequest request, CancellationToken cancellationToken = default);

    Task<PagedResult<ProductSubscriptionPlan>> GetProductSubscriptionPlans(string productIdOrHandle, int? page = 1, int? pageSize = 10, CancellationToken cancellationToken = default);

    Task<Product?> FindProductVariantByAttributes(string productIdOrHandle, Dictionary<string, string?> selectedAttributeValues, CancellationToken cancellationToken = default);

    Task<IEnumerable<Product>> FindProductVariantsByAttributes(string productIdOrHandle, Dictionary<string, string?> selectedAttributeValues, CancellationToken cancellationToken = default);

    Task<PagedResult<Product>> GetProductVariants(string productIdOrHandle, int page = 1, int pageSize = 10, string? searchString = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<AttributeValue>> GetAvailableProductVariantAttributesValues(string productIdOrHandle, string attributeId, Dictionary<string, string?> selectedAttributeValues, CancellationToken cancellationToken = default);
}