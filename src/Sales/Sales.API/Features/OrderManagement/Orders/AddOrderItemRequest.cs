namespace YourBrand.Sales.API.Features.OrderManagement.Orders;

public sealed record AddOrderItemRequest(string Description, string? ItemId, Guid? SubscriptionPlanId, double Quantity, string? Unit, decimal UnitPrice, decimal? RegularPrice, double VatRate, decimal? Discount, string? Notes);

public record UpdateOrderItemRequest(string Description, string? ItemId, Guid? SubscriptionPlanId, double Quantity, string? Unit, decimal UnitPrice, decimal? RegularPrice, double VatRate, decimal? Discount, string? Notes);

public record UpdateOrderItemQuantityRequest(string Description, string? ItemId, double Quantity);