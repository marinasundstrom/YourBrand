
using YourBrand.StoreFront;

namespace BlazorApp.Brands;

public interface IBrandsService
{
    Task<PagedResult<Brand>> GetBrands(string? productCategoryId = null, int? page = 1, int? pageSize = 10, string? searchTerm = null, CancellationToken cancellationToken = default);
}

public sealed class BrandsService(IBrandsClient brandsClient) : IBrandsService
{
    public async Task<PagedResult<Brand>> GetBrands(string? productCategoryId = null, int? page = 1, int? pageSize = 10, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
        var results = await brandsClient.GetBrandsAsync(productCategoryId, page, pageSize, searchTerm, cancellationToken);
        return new PagedResult<Brand>(results.Items.Select(brand => brand.Map()), results.Total);
    }
}