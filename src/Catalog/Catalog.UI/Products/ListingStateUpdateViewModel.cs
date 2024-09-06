using System.ComponentModel.DataAnnotations;

using MudBlazor;

namespace YourBrand.Catalog.Products;

public class ListingStateUpdateViewModel(IProductsClient productsClient, ISnackbar snackbar)
{
    public static ListingStateUpdateViewModel Create(Product product, IProductsClient productsClient, ISnackbar snackbar)
    {
        return new(productsClient, snackbar)
        {
            OrganizationId = product.OrganizationId,
            ProductId = product.Id,
            ListingState = product.ListingState
        };
    }

    [Required]
    public ProductListingState ListingState { get; set; }

    public string OrganizationId { get; private set; }

    public int ProductId { get; private set; }

    public async Task UpdateListingState()
    {
        try
        {
            await productsClient.UpdateProductListingStateAsync(OrganizationId, ProductId.ToString(), new UpdateProductListingStateRequest()
            {
                ListingState = ListingState
            });

            snackbar.Add("Visibility was updated", Severity.Info);
        }
        catch
        {
            snackbar.Add("Failed to update product visibility", Severity.Error);
        }
    }
}