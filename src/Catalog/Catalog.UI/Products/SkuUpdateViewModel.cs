using System.ComponentModel.DataAnnotations;

using MudBlazor;

namespace YourBrand.Catalog.Products;

public class SkuUpdateViewModel(IProductsClient productsClient, ISnackbar snackbar)
{
    public static SkuUpdateViewModel Create(Product product, IProductsClient productsClient, ISnackbar snackbar)
    {
        return new(productsClient, snackbar)
        {
            ProductId = product.Id,
            Sku = product.Sku
        };
    }

    [Required]
    public string Sku { get; set; }

    public long ProductId { get; private set; }

    public async Task UpdateSku()
    {
        try
        {
            await productsClient.UpdateProductSkuAsync(ProductId.ToString(), new UpdateProductSkuRequest()
            {
                Sku = Sku
            });

            snackbar.Add("Sku was updated", Severity.Info);
        }
        catch
        {
            snackbar.Add("Failed to update product handle", Severity.Error);
        }
    }
}