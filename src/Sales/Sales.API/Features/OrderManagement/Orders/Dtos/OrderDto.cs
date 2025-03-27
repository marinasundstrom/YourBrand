namespace YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Features.OrderManagement.Users;
using YourBrand.Sales.Features.SubscriptionManagement;

public sealed record OrderDto(
    string Id,
    int? OrderNo,
    DateTimeOffset? Date,
    OrderTypeDto Type,
    ParentOrderDto? Parent,
    OrderStatusDto Status,
    UserDto? AssigneeId,
    CustomerDto? Customer,
    string Currency,
    string? Reference,
    string? Note,
    SubscriptionDto? Subscription,
    OrderScheduleDto? Schedule,
    BillingDetailsDto? BillingDetails,
    ShippingDetailsDto? ShippingDetails,
    IEnumerable<OrderItemDto> Items,
    decimal SubTotal,
    IEnumerable<OrderVatAmountDto> VatAmounts,
    double? VatRate,
    decimal Vat,
    IEnumerable<DiscountDto> Discounts,
    decimal? Discount,
    decimal Total,
    DateTimeOffset Created,
    UserDto? CreatedBy,
    DateTimeOffset? LastModified,
    UserDto? LastModifiedBy);

public record OrderScheduleDto(
    DateTimeOffset? PlannedStartDate,
    DateTimeOffset? PlannedEndDate,
    DateTimeOffset? ActualStartDate,
    DateTimeOffset? ActualEndDate
);

public record ParentOrderDto(
    string Id,
    int? OrderNo,
    DateTimeOffset? Date
);

public record OrderShortDto(
    string Id,
    int? OrderNo,
    DateTimeOffset? Date
);

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

public sealed record OrderItemDto(
    string Id,
    ProductType ProductType,
    string Description,
    string? ProductId,
    string? SKU,
    SubscriptionPlanDto? SubscriptionPlan,
    SubscriptionDto? Subscription,
    decimal UnitPrice,
    string? Unit,
    IEnumerable<OrderItemOptionDto> Options,
    IEnumerable<DiscountDto> Discounts,
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

public sealed record DiscountDto(
    string Description,
    double? Rate,
    decimal? Amount,
    decimal? Total,
    DateTimeOffset? EffectiveDate,
    DateTimeOffset? ExpiryDate);

public sealed record OrderItemOptionDto(
    string Id,
    string Name,
    string? Description,
    string? Value,
    string? ProductId,
    string? ItemId,
    decimal? Price,
    decimal? Discount,
    DateTimeOffset Created,
    UserDto? CreatedBy,
    DateTimeOffset? LastModified,
    UserDto? LastModifiedBy);