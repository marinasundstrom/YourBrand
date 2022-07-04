namespace YourBrand.Orders.Models;

using System.Runtime.Serialization;

using Newtonsoft.Json;

using YourBrand.Orders.Hypermedia;

public class Orders : Resource<OrdersEmbedded>
{
    public int Count { get; set; }

    public int Total { get; set; }
}

public class OrdersEmbedded
{
    [JsonProperty("orders", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<Order>? Orders { get; set; }
}

public class Order : Resource<OrderEmbedded>
{
    public Guid Id { get; set; }
    public int OrderNo { get; set; }
    //public YourBrand.Orders.Contracts.OrderType Type { get; set; }
    public DateTime Date { get; set; }
    public string Status { get; set; } = null!;

    public decimal? SubTotal { get; set; }

    [JsonProperty("vat", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Vat { get; set; }

    [JsonProperty("vatRate", NullValueHandling = NullValueHandling.Ignore)]
    public double? VatRate { get; set; }

    [JsonProperty("discount", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Discount { get; set; }

    [JsonProperty("charge", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Charge { get; set; }

    [JsonProperty("rounding", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Rounding { get; set; }
    public decimal Total { get; set; }

    [JsonProperty("totals", NullValueHandling = NullValueHandling.Ignore)]
    public Dictionary<string, OrderTotal>? Totals { get; set; }

    //public YourBrand.Orders.Contracts.PaymentDetailsDto? Payment { get; set; }

    //public string? Signature { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object>? CustomFields { get; set; }
}

public class OrderEmbedded
{
    [JsonProperty("items", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<OrderItem>? Items { get; set; }

    [JsonProperty("discounts", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<OrderDiscount>? Discounts { get; set; }

    [JsonProperty("charges", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<OrderCharge>? Charges { get; set; }

    [JsonProperty("customFields", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<CustomField>? CustomFields { get; set; }
}

public class OrderItem : Resource<OrderItemEmbedded>
{
    public Guid Id { get; set; }
    public string Description { get; set; } = null!;
    public string? ItemId { get; set; }
    public string? Unit { get; set; }
    public decimal Price { get; set; }
    public double VatRate { get; set; }
    public double Quantity { get; set; }

    [JsonProperty("discount", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Discount { get; set; }

    [JsonProperty("charge", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Charge { get; set; }

    public decimal Total { get; set; }

    [JsonExtensionData]
    public Dictionary<string, object>? CustomFields { get; set; }
}

public class OrderItemEmbedded
{
    [JsonProperty("discounts", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<OrderDiscount>? Discounts { get; set; }

    [JsonProperty("charges", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<OrderCharge>? Charges { get; set; }

    [JsonProperty("customFields", NullValueHandling = NullValueHandling.Ignore)]
    public IEnumerable<CustomField>? CustomFields { get; set; }
}

public class OrderTotal
{
    public decimal SubTotal { get; set; }

    public decimal Vat { get; set; }

    public decimal Total { get; set; }
}

public class OrderDiscount : Resource
{
    public Guid Id { get; set; }

    public string Description { get; set; } = null!;

    [JsonProperty("discountId", NullValueHandling = NullValueHandling.Ignore)]
    public Guid? DiscountId { get; set; }

    [JsonProperty("amount", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Amount { get; set; }

    [JsonProperty("percent", NullValueHandling = NullValueHandling.Ignore)]
    public double? Percent { get; set; }

    [JsonProperty("quantity", NullValueHandling = NullValueHandling.Ignore)]
    public int? Quantity { get; set; }

    public decimal Total { get; set; }
}

public class OrderCharge : Resource
{
    public Guid Id { get; set; }

    public string Description { get; set; } = null!;

    [JsonProperty("chargeId", NullValueHandling = NullValueHandling.Ignore)]
    public Guid? ChargeId { get; set; }

    [JsonProperty("amount", NullValueHandling = NullValueHandling.Ignore)]
    public decimal? Amount { get; set; }

    [JsonProperty("percent", NullValueHandling = NullValueHandling.Ignore)]
    public double? Percent { get; set; }

    [JsonProperty("quantity", NullValueHandling = NullValueHandling.Ignore)]
    public int? Quantity { get; set; }

    public decimal Total { get; set; }
}

public class CustomField : Resource
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Value { get; set; } = null!;
}