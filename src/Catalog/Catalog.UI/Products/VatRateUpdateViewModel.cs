using MudBlazor;

namespace YourBrand.Catalog.Products;

public class VatRateUpdateViewModel(IProductsClient productsClient, IVatRatesClient vatRatesClient, ISnackbar snackbar)
{
    public static VatRateUpdateViewModel Create(Product product, IProductsClient productsClient, IVatRatesClient vatRatesClient, ISnackbar snackbar)
    {
        return new(productsClient, vatRatesClient, snackbar)
        {
            ProductId = product.Id,
            VatRateId = product.VatRateId
        };
    }

    public async Task InitializeAsync()
    {
        var result = await vatRatesClient.GetVatRatesAsync(null, null, null, null, null);
        VatRates = result.Items;

        VatRate = VatRates.FirstOrDefault(x => x.Id == VatRateId);
    }

    public int? VatRateId { get; set; }

    public VatRate? VatRate { get; set; }

    public IEnumerable<VatRate> VatRates { get; set; }

    public long ProductId { get; private set; }

    public async Task UpdateVatRate()
    {
        try
        {
            await productsClient.UpdateProductVatRateAsync(ProductId.ToString(), new UpdateProductVatRateRequest()
            {
                VatRateId = VatRate?.Id
            });

            snackbar.Add("Vat Rate was updated", Severity.Info);
        }
        catch
        {
            snackbar.Add("Failed to update product Vat Rate", Severity.Error);
        }
    }
}