using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;
using YourBrand.Sales.API.Features.OrderManagement.Domain.ValueObjects;
using YourBrand.Sales.API.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.API.Features.OrderManagement.Users;

namespace YourBrand.Sales.API.Features.OrderManagement.Orders;

public static class Mappings
{
    public static OrderDto ToDto(this Order order) => new OrderDto(order.Id, order.OrderNo, order.Date, order.Status.ToDto(), order.Assignee?.ToDto(), order.CustomerId, order.Currency,
        order.BillingDetails?.ToDto(),
        order.ShippingDetails?.ToDto(),
        order.Items.Select(x => x.ToDto()), order.SubTotal,
        order.VatAmounts.Select(x => new OrderVatAmountDto(x.Name, x.Rate, x.SubTotal, x.Vat, x.Total)),
        order.Vat.GetValueOrDefault(),
        order.Discounts.Select(x => new OrderDiscountDto(x.Amount, x.Description)),
        order.Discount, order.Total, order.Created, order.CreatedBy?.ToDto(), order.LastModified, order.LastModifiedBy?.ToDto());

    public static OrderItemDto ToDto(this OrderItem orderItem) => new(
        orderItem.Id,
        orderItem.Description,
        orderItem.ItemId,
        orderItem.Quantity,
        orderItem.Unit,
        orderItem.Price,
        orderItem.RegularPrice,
        orderItem.VatRate,
        orderItem.Discount,
        orderItem.Total,
        orderItem.Notes,
        orderItem.Created,
        orderItem.CreatedBy?.ToDto(),
        orderItem.LastModified,
        orderItem.LastModifiedBy?.ToDto());

    public static OrderStatusDto ToDto(this OrderStatus orderStatus) => new(orderStatus.Id, orderStatus.Name, orderStatus.Handle, orderStatus.Description);

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