using System.ComponentModel.DataAnnotations;

using MudBlazor;

using YourBrand.AppService.Client;

namespace YourBrand.Catalog.Products;

public class HandleUpdateViewModel(IProductsClient productsClient, ISnackbar snackbar)
{
    public static HandleUpdateViewModel Create(Product product, IProductsClient productsClient, ISnackbar snackbar)
    {
        return new(productsClient, snackbar)
        {
            OrganizationId = product.OrganizationId,
            ProductId = product.Id,
            Handle = product.Handle
        };
    }

    public string OrganizationId { get; set; }

    [Required]
    public string Handle { get; set; }

    public int ProductId { get; private set; }

    public async Task UpdateHandle()
    {
        try
        {
            await productsClient.UpdateProductHandleAsync(OrganizationId, ProductId.ToString(), new UpdateProductHandleRequest()
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