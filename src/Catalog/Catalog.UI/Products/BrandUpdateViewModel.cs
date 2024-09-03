using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Components;

using MudBlazor;

using YourBrand.Portal.Services;

namespace YourBrand.Catalog.Products;

public class BrandUpdateViewModel(IProductsClient productsClient, IBrandsClient brandsClient, ISnackbar snackbar, IStoreProvider storeProvider)
{
    public static BrandUpdateViewModel Create(Product product, IProductsClient productsClient, IBrandsClient brandsClient, ISnackbar snackbar, IStoreProvider storeProvider)
    {
        return new(productsClient, brandsClient, snackbar, storeProvider)
        {
            OrganizationId = product.OrganizationId,
            ProductId = product.Id,
            Brand = product.Brand
        };
    }

    [Required]
    public Brand? Brand { get; set; }

    public string OrganizationId { get; private set; }

    public long ProductId { get; private set; }

    public async Task<IEnumerable<Brand>> Search(string value)
    {
        var store = storeProvider.CurrentStore;

        var result = await brandsClient.GetBrandsAsync(OrganizationId, null, 1, 20, value, null, null);

        return result.Items;
    }

    public async Task UpdateBrand()
    {
        try
        {
            await productsClient.UpdateProductBrandAsync(OrganizationId, ProductId.ToString(), new UpdateProductBrandRequest()
            {
                BrandId = Brand.Id
            });

            snackbar.Add("Brand was updated", Severity.Info);
        }
        catch
        {
            snackbar.Add("Failed to update product brand", Severity.Error);
        }
    }

    public record ProductBrand(long Id, string Name, bool CanAddProducts);
}