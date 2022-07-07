using System.ComponentModel.DataAnnotations;

using YourBrand.Catalog.Client;

namespace YourBrand.Orders;

public class OrderTotalVM
{
    public decimal SubTotal { get; set; }

    public string? VatRate { get; set; }

    public decimal Vat { get; set; }

    public decimal Total { get; set; }
}

public class OrderItemVM
{
    public Guid? Id { get; set; }

    [Required]
    public ProductDto? Item { get; set; }

    public int Quantity { get; set; }

    public List<ChargeVM> Charges { get; set; } = new List<ChargeVM>();

    public List<DiscountVM> Discounts { get; set; } = new List<DiscountVM>();
}

public class ChargeVM
{
    public Guid? Id { get; set; }

    [Required]
    public string? Description { get; set; }

    public decimal Total { get; set; }
}

public class DiscountVM
{
    public Guid? Id { get; set; }

    [Required]
    public string? Description { get; set; }

    public decimal Total { get; set; }
}