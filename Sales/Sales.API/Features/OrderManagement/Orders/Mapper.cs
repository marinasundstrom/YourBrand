using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;
using YourBrand.Sales.API.Features.OrderManagement.Domain.ValueObjects;
using YourBrand.Sales.API.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.API.Features.OrderManagement.Users;
using YourBrand.Sales.Features.Subscriptions;

namespace YourBrand.Sales.API.Features.OrderManagement.Orders;

public static class Mappings
{
    public static OrderDto ToDto(this Order order) => new (
        order.Id,
        order.OrderNo,
        order.Date,
        order.Parent?.ToParentDto(),
        order.Status.ToDto(),
        order.Assignee?.ToDto(),
        order.Customer?.ToDto(),
        order.Currency,
        order.Reference,
        order.Notes,
        order.Subscription?.ToDto(),
        order.PlannedStartDate,
        order.PlannedEndDate,
        order.ActualStartDate,
        order.ActualEndDate,
        order.BillingDetails?.ToDto(),
        order.ShippingDetails?.ToDto(),
        order.Items.Select(x => x.ToDto()), 
        order.SubTotal,
        order.VatAmounts.Select(x => new OrderVatAmountDto(x.Name, x.Rate, x.SubTotal, x.Vat, x.Total)),
        order.Vat.GetValueOrDefault(),
        order.Discounts.Select(x => new OrderDiscountDto(x.Amount, x.Description)),
        order.Discount, 
        order.Total, 
        order.Created, 
        order.CreatedBy?.ToDto(), 
        order.LastModified, 
        order.LastModifiedBy?.ToDto());

    public static ParentOrderDto ToParentDto(this Order order) => new (
        order.Id,
        order.OrderNo,
        order.Date);

    public static OrderShortDto ToShortDto(this Order order) => new (
        order.Id,
        order.OrderNo,
        order.Date);

    public static OrderItemDto ToDto(this OrderItem orderItem) => new(
        orderItem.Id,
        orderItem.ProductType,
        orderItem.Description,
        orderItem.ProductId,
        orderItem.Sku,
        orderItem.SubscriptionPlan?.ToDto(),
        orderItem.Subscription?.ToDto(),
        orderItem.Price,
        orderItem.Unit,    
        orderItem.Discount,
        orderItem.RegularPrice,
        orderItem.VatRate,
        orderItem.Quantity,
        orderItem.Vat,
        orderItem.Total,
        orderItem.Notes,
        orderItem.Created,
        orderItem.CreatedBy?.ToDto(),
        orderItem.LastModified,
        orderItem.LastModifiedBy?.ToDto());

    public static OrderStatusDto ToDto(this OrderStatus orderStatus) => new(orderStatus.Id, orderStatus.Name, orderStatus.Handle, orderStatus.Description);

    public static CustomerDto ToDto(this Customer customer) => new CustomerDto(customer.Id, customer.CustomerNo, customer.Name);

    public static UserDto ToDto(this User user) => new(user.Id, user.Name);

    public static UserInfoDto ToDto2(this User user) => new(user.Id, user.Name);

    public static AddressDto ToDto(this Domain.ValueObjects.Address address) => new()
    {
        Thoroughfare = address.Thoroughfare,
        Premises = address.Premises,
        SubPremises = address.SubPremises,
        PostalCode = address.PostalCode,
        Locality = address.Locality,
        SubAdministrativeArea = address.SubAdministrativeArea,
        AdministrativeArea = address.AdministrativeArea,
        Country = address.Country
    };

    public static BillingDetailsDto ToDto(this BillingDetails billingDetails) => new()
    {
        FirstName = billingDetails.FirstName,
        LastName = billingDetails.LastName,
        SSN = billingDetails.SSN,
        Email = billingDetails.Email,
        PhoneNumber = billingDetails.PhoneNumber,
        Address = billingDetails.Address?.ToDto()
    };

    public static ShippingDetailsDto ToDto(this ShippingDetails billingDetails) => new()
    {
        FirstName = billingDetails.FirstName,
        LastName = billingDetails.LastName,
        Address = billingDetails.Address?.ToDto()
    };

    public static CurrencyAmountDto ToDto(this CurrencyAmount currencyAmount) => new(currencyAmount.Currency, currencyAmount.Amount);

}