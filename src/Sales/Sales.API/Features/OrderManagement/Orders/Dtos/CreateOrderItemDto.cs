namespace YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

public sealed class CreateOrderItemDto
{
    public string Description { get; set; } = null!;

    public string? ItemId { get; set; }

    public Guid? SubscriptionPlanId { get; set; }

    public double Quantity { get; set; }

    public string? Unit { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal? RegularPrice { get; set; }

    public List<CreateOrderItemOptionDto> Options { get; set; } = new List<CreateOrderItemOptionDto>();

    public List<CreateDiscountDto> Discounts { get; set; } = new List<CreateDiscountDto>();

    public double? VatRate { get; set; }

    public string? Notes { get; set; }
}

public class CreateDiscountDto
{
    public string Description { get; set; } = null!;

    public double? Rate { get; set; }

    public decimal? Amount { get; set; }

    public decimal? Total { get; set; }

}

public class CreateOrderItemOptionDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public string? Value { get; set; } = null!;

    public string? ProductId { get; set; }

    public string? ItemId { get; set; }

    public decimal? Price { get; set; }
}