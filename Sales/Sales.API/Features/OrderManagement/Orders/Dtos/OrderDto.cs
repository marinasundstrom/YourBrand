namespace YourBrand.Sales.API.Features.OrderManagement.Orders.Dtos;

using YourBrand.Sales.API.Features.OrderManagement.Users;

public sealed record OrderDto(string Id, int OrderNo, DateTime Date, OrderStatusDto Status, UserDto? AssigneeId, string? CustomerId, string Currency, BillingDetailsDto? BillingDetails, ShippingDetailsDto? ShippingDetails, IEnumerable<OrderItemDto> Items, decimal SubTotal, IEnumerable<OrderVatAmountDto> VatAmounts, decimal Vat, IEnumerable<OrderDiscountDto> Discounts, decimal? Discount, decimal Total, DateTimeOffset Created, UserDto? CreatedBy, DateTimeOffset? LastModified, UserDto? LastModifiedBy);

public sealed record OrderVatAmountDto(string Name, double VatRate, decimal SubTotal, decimal? Vat, decimal Total);

public sealed record OrderDiscountDto(decimal Amount, string Description);

public sealed record OrderItemDto(string Id, string Description, string? ItemId, double Quantity, string? Unit, decimal UnitPrice, decimal? RegularPrice, double? VatRate, decimal? Discount, decimal Total, string? Notes, DateTimeOffset Created, UserDto? CreatedBy, DateTimeOffset? LastModified, UserDto? LastModifiedBy);