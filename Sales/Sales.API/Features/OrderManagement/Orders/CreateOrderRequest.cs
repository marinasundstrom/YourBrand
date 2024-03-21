using YourBrand.Sales.API.Features.OrderManagement.Orders.Commands;
using YourBrand.Sales.API.Features.OrderManagement.Orders.Dtos;

namespace YourBrand.Sales.API.Features.OrderManagement.Orders;

public sealed record CreateOrderRequest(int? Status, SetCustomerDto? Customer, BillingDetailsDto BillingDetails, ShippingDetailsDto? ShippingDetails, IEnumerable<CreateOrderItemDto> Items);

public sealed record CreateDraftOrderRequest();