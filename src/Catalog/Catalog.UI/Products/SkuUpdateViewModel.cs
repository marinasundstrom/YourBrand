using System.ComponentModel.DataAnnotations;

using MudBlazor;

using YourBrand.AppService.Client;

namespace YourBrand.Catalog.Products;

public class SkuUpdateViewModel(IProductsClient productsClient, ISnackbar snackbar)
{
    public static SkuUpdateViewModel Create(Product product, IProductsClient productsClient, ISnackbar snackbar)
    {
        return new(productsClient, snackbar)
        {
            OrganizationId = product.OrganizationId,
            ProductId = product.Id,
            Sku = product.Sku
        };
    }

    public string OrganizationId { get; set; }

    [Required]
    public string Sku { get; set; }

    public int ProductId { get; private set; }

    public async Task UpdateSku()
    {
        try
        {
            await productsClient.UpdateProductSkuAsync(OrganizationId, ProductId.ToString(), new UpdateProductSkuRequest()
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