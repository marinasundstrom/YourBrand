using System.ComponentModel.DataAnnotations;

using Core;

namespace YourBrand.Sales.OrderManagement;

public class OrderItemViewModel
{
    public string? Id { get; set; }

    [Required]
    public string Description { get; set; } = null!;

    public string? ItemId { get; set; }

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

    public decimal SubTotal => Total - Vat;

    public decimal Vat => Math.Round(Total.GetVatFromTotal(VatRate.GetValueOrDefault()), 2, MidpointRounding.AwayFromZero);

    public decimal Total => Math.Round(Price * (decimal)Quantity, 2, MidpointRounding.AwayFromZero);

    public string? Notes { get; set; }
}
