using System.ComponentModel.DataAnnotations;

using MudBlazor;

using YourBrand.Portal.Services;
using YourBrand.Catalog;

namespace YourBrand.Catalog.Products;

public class CategoryUpdateViewModel(IProductsClient productsClient, IProductCategoriesClient productCategoriesClient, ISnackbar snackbar, IStoreProvider storeProvider)
{
    public static CategoryUpdateViewModel Create(Product product, IProductsClient productsClient, IProductCategoriesClient productCategoriesClient, ISnackbar snackbar, IStoreProvider storeProvider)
    {
        return new(productsClient, productCategoriesClient, snackbar, storeProvider)
        {
            ProductId = product.Id,
            Category = new ProductCategory(product.Category.Id!, product.Category.Name, true)
        };
    }

    [Required]
    public ProductCategory Category { get; set; }

    public long ProductId { get; private set; }

    public async Task<IEnumerable<ProductCategory>> Search(string value)
    {
        var store = storeProvider.CurrentStore;

        var result = await productCategoriesClient.GetProductCategoriesAsync(store.Id, null, true, true, 1, 20, value, null, null);

        return result.Items
            .Where(x => x.CanAddProducts)
            .Select(x => new ProductCategory(x.Id!, x.Name, x.CanAddProducts));
    }

    public async Task UpdateCategory()
    {
        try
        {
            await productsClient.UpdateProductCategoryAsync(ProductId.ToString(), new UpdateProductCategoryRequest()
            {
                ProductCategoryId = Category.Id
            });

            snackbar.Add("Category was updated", Severity.Info);
        }
        catch
        {
            snackbar.Add("Failed to update product category", Severity.Error);
        }
    }

    public record ProductCategory(long Id, string Name, bool CanAddProducts);
}