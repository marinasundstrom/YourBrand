namespace BlazorApp.ProductCategories;

public sealed class ProductCategoryService(YourBrand.StoreFront.IProductCategoriesClient productCategoriesClient) : IProductCategoryService
{
    public async Task<ProductCategoryTreeRootDto> GetProductCategoryTree(string? rootNodeIdOrPath = null, CancellationToken cancellationToken = default)
    {
        var treeRoot = await productCategoriesClient.GetProductCategoryTreeAsync(rootNodeIdOrPath, cancellationToken);
        return new ProductCategoryTreeRootDto(treeRoot.Categories.Select(x => x.ToProductCategoryTreeNodeDto()), treeRoot.ProductsCount);
    }

    public async Task<ProductCategoryDto> GetProductCategoryById(string productCategoryId, CancellationToken cancellationToken = default)
    {
        var productCategory = await productCategoriesClient.GetProductCategoryByIdAsync(productCategoryId, cancellationToken);
        return productCategory.ToDto();
    }
}

public static class Mapping
{
    public static ProductCategoryDto ToDto(this YourBrand.StoreFront.ProductCategory productCategory)
    {
        return new(productCategory.Id, productCategory.Name, productCategory.Description ?? string.Empty, productCategory.Handle, productCategory.Path, null, productCategory.ProductsCount);
    }

    public static ProductCategoryTreeNodeDto ToProductCategoryTreeNodeDto(this YourBrand.StoreFront.ProductCategoryTreeNode productCategory)
    {
        return new(productCategory.Id, productCategory.Name, productCategory.Handle, productCategory.Path, productCategory.Description, productCategory.Parent?.ToParentDto2(), productCategory.SubCategories.Select(x => x.ToProductCategoryTreeNodeDto()), productCategory.ProductsCount, productCategory.CanAddProducts);
    }

    public static ProductCategoryParent ToParentDto2(this YourBrand.StoreFront.ProductCategoryParent productCategory)
    {
        return new(productCategory.Id, productCategory.Name, productCategory.Handle, productCategory.Path, productCategory.Parent?.ToParentDto2(), productCategory.ProductsCount);
    }

    public static ProductCategoryParent ToParentDto3(this YourBrand.StoreFront.ProductCategory productCategory)
    {
        return new(productCategory.Id, productCategory.Name, productCategory.Handle, productCategory.Path, productCategory.Parent?.ToParentDto2(), productCategory.ProductsCount);
    }
}