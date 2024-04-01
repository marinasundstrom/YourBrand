using System.ComponentModel.DataAnnotations;

using Core;

using YourBrand.Catalog;

namespace YourBrand.Sales.OrderManagement;

public class OrderItemViewModel
{
    public string? Id { get; set; }

    public ProductType ProductType { get; set; }

    [Required]
    public string Description { get; set; } = null!;

    public Product? Product { get; set; }

    public string? ItemId { get; set; }

    public SubscriptionPlan? SubscriptionPlan { get; set; }

    public string? Sku { get; set; }

    [Required]
    public decimal Price { get; set; }

    public decimal? RegularPrice { get; set; }

    public decimal? Discount => RegularPrice is null ? null : RegularPrice.GetValueOrDefault() - Price;

    [Required]
    [Range(0.0001, double.MaxValue)]
    public double Quantity { get; set; } = 1;

    [Required]
    public string Unit { get; set; } = string.Empty;

    public double? VatRate { get; set; } = 0.25;

    public decimal SubTotal => Total.GetSubTotal(VatRate.GetValueOrDefault());

    public decimal Vat => Math.Round(Total.GetVatFromTotal(VatRate.GetValueOrDefault()), 2, MidpointRounding.AwayFromZero);

    public decimal Total => Math.Round(Price * (decimal)Quantity, 2, MidpointRounding.AwayFromZero);

    public string? Notes { get; set; }
}