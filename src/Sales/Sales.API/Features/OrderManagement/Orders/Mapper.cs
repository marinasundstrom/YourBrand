using YourBrand.Domain;
using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.ValueObjects;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Organizations;
using YourBrand.Sales.Features.OrderManagement.Users;
using YourBrand.Sales.Features.SubscriptionManagement;

namespace YourBrand.Sales.Features.OrderManagement.Orders;

public static class Mappings
{
    public static OrderDto ToDto(this Order order) => new(
        order.Id,
        order.OrderNo,
        order.Date,
        order.Type.ToDto(),
        order.Parent?.ToParentDto(),
        order.Status.ToDto(),
        order.Assignee?.ToDto(),
        order.Customer?.ToDto(),
        order.Currency,
        order.Reference,
        order.Notes,
        order.Subscription?.ToDto(),
        order.Schedule?.ToDto(),
        order.BillingDetails?.ToDto(),
        order.ShippingDetails?.ToDto(),
        order.Items.Select(x => x.ToDto()),
        order.SubTotal,
        order.VatAmounts.Select(x => new OrderVatAmountDto(x.Name, x.VatRate, x.SubTotal, x.Vat, x.Total)),
        order.VatRate,
        order.Vat.GetValueOrDefault(),
        order.Discounts.Select(x => x.ToDto()),
        order.Discount,
        order.Total,
        order.Created,
        order.CreatedBy?.ToDto(),
        order.LastModified,
        order.LastModifiedBy?.ToDto());


    public static OrderScheduleDto ToDto(this OrderSchedule schedule) => new(
        schedule.PlannedStartDate,
        schedule.PlannedEndDate,
        schedule.ActualStartDate,
        schedule.ActualEndDate);

    public static DiscountDto ToDto(this Discount discount) => new(
        discount.Description,
        discount.Rate,
        discount.Amount,
        discount.Total,
        discount.EffectiveDate,
        discount.ExpiryDate);

    public static ParentOrderDto ToParentDto(this Order order) => new(
        order.Id,
        order.OrderNo,
        order.Date);

    public static OrderShortDto ToShortDto(this Order order) => new(
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
        orderItem.Options.Select(x => x.ToDto()),
        orderItem.PromotionalDiscounts.Select(x => x.ToDto()),
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

    public static OrderItemOptionDto ToDto(this OrderItemOption orderItemOption) => new(
        orderItemOption.Id, 
        orderItemOption.Name,
        orderItemOption.Description, 
        orderItemOption.Value,
        orderItemOption.ProductId, 
        orderItemOption.ItemId, 
        orderItemOption.Price, 
        orderItemOption.Discount, 
        orderItemOption.Created,
        orderItemOption.CreatedBy?.ToDto(),
        orderItemOption.LastModified,
        orderItemOption.LastModifiedBy?.ToDto());

    public static OrderTypeDto ToDto(this OrderType orderType) => new(orderType.Id, orderType.Name, orderType.Handle, orderType.Description);

    public static OrderStatusDto ToDto(this OrderStatus orderStatus) => new(orderStatus.Id, orderStatus.Name, orderStatus.Handle, orderStatus.Description);

    public static CustomerDto ToDto(this Customer customer) => new CustomerDto(customer.Id, customer.CustomerNo, customer.Name);

    public static UserDto ToDto(this User user) => new(user.Id, user.Name);

    public static UserInfoDto ToDto2(this User user) => new(user.Id, user.Name);

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


    public static OrganizationDto ToDto(this Organization user) => new(user.Id, user.Name);

    public static OrganizationDto ToDto2(this Organization user) => new(user.Id, user.Name);
}