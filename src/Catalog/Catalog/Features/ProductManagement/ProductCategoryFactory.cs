using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Persistence;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Features.ProductManagement;

public class ProductCategoryFactory(CatalogContext catalogContext, ITenantContext tenantContext)
{
    public async Task<ProductCategory> CreateProductCategoryAsync(string organizationId, ProductCategoryOptions options, CancellationToken cancellationToken = default)
    {
        int categoryId = 1;

        try
        {
            categoryId = await catalogContext.ProductCategories
                .Where(x => x.OrganizationId == organizationId)
                .Where(x => x.StoreId == options.StoreId)
                .MaxAsync(x => x.Id, cancellationToken) + 1;
        }
        catch { }

        var productCategory = new ProductCategory(categoryId) {
            OrganizationId = organizationId,
            Name = options.Name,
            Handle = options.Handle,
            Description = options.Description,
            Path = options.Path,
            StoreId = options.StoreId,
            CanAddProducts = options.CanAddProducts
        };

        catalogContext.ProductCategories.Add(productCategory);

        await catalogContext.SaveChangesAsync(cancellationToken);

        return productCategory;
    }
}

public class ProductCategoryOptions
{
    public string Name { get; set; }
    public string OrganizationId { get; set; }
    public string Handle { get; set; }
    public string Path { get; set; }
    public string? Description { get; set; }
    public string StoreId { get; set; }
    public bool CanAddProducts { get; set; }
}