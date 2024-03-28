using System.ComponentModel.DataAnnotations;

using MudBlazor;

using YourBrand.Catalog;

namespace YourBrand.Catalog.Products;

public class DetailsUpdateViewModel(IProductsClient productsClient, ISnackbar snackbar)
{
    public static DetailsUpdateViewModel Create(Product product, IProductsClient productsClient, ISnackbar snackbar)
    {
        return new(productsClient, snackbar)
        {
            ProductId = product.Id,
            Name = product.Name,
            ShadowName = product.Name,
            Description = product.Description,
            CanInheritProperties = product.Parent is not null
        };
    }

    public long ProductId { get; init; }

    public string Name { get; set; }

    [Required]
    public string ShadowName { get; set; }

    [Required]
    public string Description { get; set; }

    public bool InheritName { get; set; }

    public bool InheritDescription { get; set; }

    public bool CanInheritProperties { get; init; }

    public async Task UpdateDetails()
    {
        try
        {
            await productsClient.UpdateProductDetailsAsync(ProductId.ToString(), new UpdateProductDetailsRequest()
            {
                Name = ShadowName,
                Description = Description,
            });

            Name = ShadowName;

            snackbar.Add("Product details were updated", Severity.Info);
        }
        catch
        {
            snackbar.Add("Failed to update product details", Severity.Error);
        }
    }
}