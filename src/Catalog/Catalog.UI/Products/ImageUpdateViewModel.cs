using Microsoft.AspNetCore.Components.Forms;

using MudBlazor;

namespace YourBrand.Catalog.Products;

public class ImageUpdateViewModel(IProductsClient productsClient, ISnackbar snackbar)
{
    public static ImageUpdateViewModel Create(Product product, IProductsClient productsClient, ISnackbar snackbar)
    {
        return new(productsClient, snackbar)
        {
            ProductId = product.Id,
            Image = product.Image.Url
        };
    }

    public string Image { get; set; }

    public long ProductId { get; private set; }

    public async Task UploadProductImage(IBrowserFile file)
    {
        try
        {
            var productImage = await productsClient.UploadProductImageAsync(ProductId.ToString(), true, new FileParameter(file.OpenReadStream(3 * 1000000), file.Name));

            Image = productImage.Url;

            snackbar.Add("Image was updated", Severity.Info);
        }
        catch
        {
            snackbar.Add("Failed to update product image", Severity.Error);
        }
    }

}