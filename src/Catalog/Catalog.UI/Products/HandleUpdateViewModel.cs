using System.ComponentModel.DataAnnotations;

using MudBlazor;

using YourBrand.Catalog;

namespace YourBrand.Catalog.Products;

public class HandleUpdateViewModel(IProductsClient productsClient, ISnackbar snackbar)
{
    public static HandleUpdateViewModel Create(Product product, IProductsClient productsClient, ISnackbar snackbar)
    {
        return new(productsClient, snackbar)
        {
            ProductId = product.Id,
            Handle = product.Handle
        };
    }

    [Required]
    public string Handle { get; set; }

    public long ProductId { get; private set; }

    public async Task UpdateHandle()
    {
        try
        {
            await productsClient.UpdateProductHandleAsync(ProductId.ToString(), new UpdateProductHandleRequest()
            {
                Handle = Handle
            });

            snackbar.Add("Handle was updated", Severity.Info);
        }
        catch
        {
            snackbar.Add("Failed to update product handle", Severity.Error);
        }
    }
}