namespace BlazorApp.ProductCategories;

public interface IProductCategoryService
{
    Task<ProductCategoryTreeRootDto> GetProductCategoryTree(string? rootNodeIdOrPath = null, CancellationToken cancellationToken = default);

    Task<ProductCategoryDto> GetProductCategoryById(string productCategoryId, CancellationToken cancellationToken = default);
}

public record class ProductCategoryDto(long Id, string Name, string Description, string Handle, string Path, ProductCategoryParent? Parent, long ProductsCount);

public record class ProductCategoryTreeRootDto(IEnumerable<ProductCategoryTreeNodeDto> Categories, long ProductsCount);

public record class ProductCategoryTreeNodeDto(long Id, string Name, string Handle, string Path, string? Description, ProductCategoryParent? Parent, IEnumerable<ProductCategoryTreeNodeDto> SubCategories, long ProductsCount, bool CanAddProducts);

public record class ProductCategoryParent(long Id, string Name, string Handle, string Path, ProductCategoryParent? Parent, long ProductsCount);