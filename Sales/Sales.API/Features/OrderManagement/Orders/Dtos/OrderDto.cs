namespace YourBrand.Sales.API.Features.OrderManagement.Orders.Dtos;

using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;
using YourBrand.Sales.API.Features.OrderManagement.Users;

public sealed record OrderDto(
    string Id, 
    int OrderNo, 
    DateTime Date,
    OrderStatusDto Status, 
    UserDto? AssigneeId, 
    CustomerDto? Customer, 
    string Currency,
    string? Reference,
    string? Note,
    BillingDetailsDto? BillingDetails, 
    ShippingDetailsDto? ShippingDetails, 
    IEnumerable<OrderItemDto> Items, 
    decimal SubTotal, 
    IEnumerable<OrderVatAmountDto> VatAmounts, 
    decimal Vat, 
    IEnumerable<OrderDiscountDto> Discounts, 
    decimal? Discount,
    decimal Total, 
    DateTimeOffset Created, 
    UserDto? CreatedBy,
    DateTimeOffset? LastModified,
    UserDto? LastModifiedBy);

public record CustomerDto(
    string Id,
    long CustomerNo,
    string Name);

public sealed record OrderVatAmountDto(
    string Name, 
    double VatRate, 
    decimal SubTotal, 
    decimal? Vat, 
    decimal Total);

public sealed record OrderDiscountDto(
    decimal Amount, 
    string Description);

public sealed record OrderItemDto(
    string Id,
    ProductType ProductType,
    string Description, 
    string? ProductId,
    string? SKU,
    decimal UnitPrice,
    string? Unit,
    decimal? Discount,
    decimal? RegularPrice,
    double? VatRate,
    double Quantity,
    decimal? Vat,
    decimal Total, 
    string? Notes, 
    DateTimeOffset Created, 
    UserDto? CreatedBy, 
    DateTimeOffset? LastModified, 
    UserDto? LastModifiedBy);