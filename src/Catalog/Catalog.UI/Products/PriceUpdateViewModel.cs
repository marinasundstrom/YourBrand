using System.ComponentModel.DataAnnotations;

using MudBlazor;

using YourBrand.AppService.Client;

namespace YourBrand.Catalog.Products;

public class PriceUpdateViewModel(IProductsClient productsClient, IDialogService dialogService, ISnackbar snackbar)
{
    public static PriceUpdateViewModel Create(Product product, IProductsClient productsClient, IDialogService dialogService, ISnackbar snackbar)
    {
        return new(productsClient, dialogService, snackbar)
        {
            OrganizationId = product.OrganizationId,
            ProductId = product.Id,
            Price = product.Price,
            RegularPrice = product.RegularPrice,
            DiscountRate = product.DiscountRate
        };
    }

    public string OrganizationId { get; set; }

    [Range(0, 100000)]
    public decimal Price { get; set; }

    public decimal? RegularPrice { get; set; }

    public double? DiscountRate { get; set; }

    public int ProductId { get; private set; }

    public async Task UpdatePrice()
    {
        try
        {
            await productsClient.UpdateProductPriceAsync(OrganizationId, ProductId.ToString(), new UpdateProductPriceRequest()
            {
                Price = Price
            });

            snackbar.Add("Price was updated", Severity.Info);
        }
        catch
        {
            snackbar.Add("Failed to update product price", Severity.Error);
        }
    }

    public async Task SetDiscountPrice()
    {
        var parameters = new DialogParameters();
        parameters.Add(nameof(SetProductDiscountPriceDialog.ProductId), ProductId);
        parameters.Add(nameof(SetProductDiscountPriceDialog.RegularPrice), Price);

        var dialogRef = dialogService.Show<SetProductDiscountPriceDialog>("Set discount price", parameters);

        var r = await dialogRef.Result;

        if (r.Canceled)
        {
            return;
        }

        RegularPrice = Price;

        (decimal newPrice, double discountRate) = ((decimal newPrice, double discountRate))r.Data;

        Price = newPrice;
        DiscountRate = discountRate;
    }

    public async Task RestoreRegularPrice()
    {
        await productsClient.RestoreProductRegularPriceAsync(OrganizationId, ProductId.ToString(), new RestoreProductRegularPriceReguest());

        Price = RegularPrice.GetValueOrDefault();
        RegularPrice = null;
        DiscountRate = null;
    }
}