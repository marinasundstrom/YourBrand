namespace YourBrand.Orders.Contracts;

public class CreateOrderDto
{
    //public OrderType OrderType { get; set; } = OrderType.Order;

    //public string? Cashier { get; set; }

    //public string? CheckoutPoint { get; set; }

    //public int? OrderNo { get; set; }

    public string? Status { get; set; }

    public string? CustomerNo { get; set; }

    public IEnumerable<CreateOrderItemDto> Items { get; set; } = null!;

    public IEnumerable<OrderChargeDto>? Charges { get; set; }

    public IEnumerable<OrderDiscountDto>? Discounts { get; set; }

    //public PaymentDetailsDto? Payment { get; set; }

    //public string? Signature { get; set; }

    public Dictionary<string, string>? CustomFields { get; set; }
}

public class CreateOrderItemDto
{
    public string? Description { get; set; } = null!;

    public string ItemId { get; set; } = null!;

    public decimal Price { get; set; }

    public double Quantity { get; set; }

    public IEnumerable<OrderChargeDto>? Charges { get; set; }

    public IEnumerable<OrderDiscountDto>? Discounts { get; set; }

    public IDictionary<string, string>? CustomFields { get; set; }
}

public class AddOrderItemDetails 
{
    public string? Description { get; set; }
    public string? ItemId { get; set; }
    public string? Unit { get; set; }
    public double Quantity { get; set; } = 1;
}  