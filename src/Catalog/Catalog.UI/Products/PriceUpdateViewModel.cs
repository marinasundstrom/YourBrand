using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using MudBlazor;

using YourBrand.AppService.Client;
using YourBrand.Catalog;

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

    public ObservableCollection<ProductPriceTier> PriceTiers { get; } = new();

    public event Func<Task>? PriceTiersChanged;

    public async Task InitializeAsync()
    {
        await LoadPriceTiers();
    }

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

    public async Task LoadPriceTiers()
    {
        try
        {
            var tiers = await productsClient.GetProductPriceTiersAsync(OrganizationId, ProductId.ToString());

            PriceTiers.Clear();

            foreach (var tier in tiers.OrderBy(x => x.FromQuantity).ThenBy(x => x.ToQuantity ?? int.MaxValue))
            {
                PriceTiers.Add(tier);
            }

            await NotifyPriceTiersChanged();
        }
        catch
        {
            snackbar.Add("Failed to load price tiers", Severity.Error);
        }
    }

    public async Task CreatePriceTier(PriceTierFormData data)
    {
        try
        {
            var created = await productsClient.CreateProductPriceTierAsync(OrganizationId, ProductId.ToString(), new CreateProductPriceTierRequest
            {
                FromQuantity = data.FromQuantity,
                ToQuantity = data.ToQuantity,
                TierType = data.TierType,
                Value = data.Value
            });

            PriceTiers.Add(created);
            await NotifyPriceTiersChanged();
            snackbar.Add("Price tier created", Severity.Success);
        }
        catch
        {
            snackbar.Add("Failed to create price tier", Severity.Error);
        }
    }

    public async Task UpdatePriceTier(ProductPriceTier tier, PriceTierFormData data)
    {
        try
        {
            var updated = await productsClient.UpdateProductPriceTierAsync(OrganizationId, ProductId.ToString(), tier.Id!, new UpdateProductPriceTierRequest
            {
                FromQuantity = data.FromQuantity,
                ToQuantity = data.ToQuantity,
                TierType = data.TierType,
                Value = data.Value
            });

            var index = PriceTiers.IndexOf(tier);
            if (index >= 0)
            {
                PriceTiers[index] = updated;
            }

            await NotifyPriceTiersChanged();
            snackbar.Add("Price tier updated", Severity.Success);
        }
        catch
        {
            snackbar.Add("Failed to update price tier", Severity.Error);
        }
    }

    public async Task DeletePriceTier(ProductPriceTier tier)
    {
        try
        {
            await productsClient.DeleteProductPriceTierAsync(OrganizationId, ProductId.ToString(), tier.Id!);

            PriceTiers.Remove(tier);
            await NotifyPriceTiersChanged();
            snackbar.Add("Price tier deleted", Severity.Info);
        }
        catch
        {
            snackbar.Add("Failed to delete price tier", Severity.Error);
        }
    }

    public async Task<PriceTierFormData?> PromptPriceTierAsync(string title, PriceTierFormData? initialData = null)
    {
        var parameters = new DialogParameters
        {
            { nameof(ProductPriceTierDialog.Title), title },
            { nameof(ProductPriceTierDialog.InitialData), initialData }
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            FullWidth = true,
            MaxWidth = MaxWidth.Small
        };

        var dialog = dialogService.Show<ProductPriceTierDialog>(title, parameters, options);
        var result = await dialog.Result;

        if (result.Canceled)
        {
            return null;
        }

        return result.Data is PriceTierFormData data ? data : null;
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

    private async Task NotifyPriceTiersChanged()
    {
        if (PriceTiersChanged is not null)
        {
            await PriceTiersChanged.Invoke();
        }
    }

    public sealed record PriceTierFormData(int FromQuantity, int? ToQuantity, ProductPriceTierType TierType, decimal Value);
}