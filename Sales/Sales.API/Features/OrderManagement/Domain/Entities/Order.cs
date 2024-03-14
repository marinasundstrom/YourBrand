using System.Collections.Generic;

using YourBrand.Sales.API;
using YourBrand.Sales.API.Features.OrderManagement.Domain.Events;
using YourBrand.Sales.API.Features.OrderManagement.Domain.ValueObjects;

using Core;

namespace YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;

public class Order : AggregateRoot<string>, IAuditable
{
    readonly HashSet<OrderItem> _items = new HashSet<OrderItem>();

    public Order() : base(Guid.NewGuid().ToString())
    {
        StatusId = 1;
    }

    public int OrderNo { get; set; }

    public string CompanyId { get; private set; } = "ACME";

    public DateTime Date { get; private set; } = DateTime.Now;

    public OrderStatus Status { get; private set; } = null!;

    public int StatusId { get; set; } = 1;

    public bool UpdateStatus(int status)
    {
        var oldStatus = StatusId;
        if (status != oldStatus)
        {
            StatusId = status;

            AddDomainEvent(new OrderUpdated(Id));
            AddDomainEvent(new OrderStatusUpdated(Id, status, oldStatus));

            return true;
        }

        return false;
    }

    public User? Assignee { get; private set; }

    public string? AssigneeId { get; private set; }

    public bool UpdateAssigneeId(string? userId)
    {
        var oldAssigneeId = AssigneeId;
        if (userId != oldAssigneeId)
        {
            AssigneeId = userId;
            //AddDomainEvent(new OrderAssignedUserUpdated(OrderNo, userId, oldAssigneeId));

            return true;
        }

        return false;
    }

    public string? CustomerId { get; set; }

    public bool VatIncluded { get; set; }

    public string Currency { get; set; } = "SEK";

    public decimal SubTotal { get; set; }

    public double VatRate { get; set; }

    public List<OrderDiscount> Discounts { get; set; } = new List<OrderDiscount>();

    public List<OrderVatAmount> VatAmounts { get; set; } = new List<OrderVatAmount>();

    public decimal? Vat { get; set; }

    public decimal Discount { get; set; }

    public decimal Total { get; set; }

    public BillingDetails? BillingDetails { get; set; }

    public ShippingDetails? ShippingDetails { get; set; }

    public IReadOnlyCollection<OrderItem> Items => _items;

    public OrderItem AddItem(
                       string description,
                       string? itemId,
                       decimal price,
                       decimal? regularPrice,
                       double? discountRate,
                       decimal? discount,
                       double quantity,
                       string? unit,
                       double? vatRate,
                       string? notes)
    {
        var orderItem = new OrderItem(description, itemId!, price, regularPrice, discountRate, discount, quantity, unit, vatRate, notes);
        _items.Add(orderItem);

        Update();

        return orderItem;
    }

    public void RemoveOrderItem(OrderItem orderItem) => _items.Remove(orderItem);

    public void Update()
    {
        UpdateVatAmounts();

        VatRate = 0.25;
        Vat = Items.Sum(x => x.Vat);
        Total = Items.Sum(x => x.Total);
        SubTotal = VatIncluded ? (Total - Vat.GetValueOrDefault()) : Total;
        Discount = Items.Sum(x => x.Discount.GetValueOrDefault());
    }

    private void UpdateVatAmounts()
    {
        VatAmounts.ForEach(x =>
        {
            x.Vat = 0;
            x.SubTotal = 0;
            x.Total = 0;
        });

        foreach (var item in Items)
        {
            item.Update();

            var vatAmount = VatAmounts.FirstOrDefault(x => x.Rate == item.VatRate);
            if (vatAmount is null)
            {
                vatAmount = new OrderVatAmount()
                {
                    Rate = item.VatRate.GetValueOrDefault(),
                    Name = $"{item.VatRate * 100}%"
                };

                VatAmounts.Add(vatAmount);
            }

            vatAmount.Vat += item.Vat.GetValueOrDefault();
            vatAmount.SubTotal += item.Total - item.Vat.GetValueOrDefault();
            vatAmount.Total += item.Total;
        }

        VatAmounts.ToList().ForEach(x =>
        {
            if (x.Vat == 0 && x.Rate != 0)
            {
                VatAmounts.Remove(x);
            }
        });
    }

    public User? CreatedBy { get; set; }

    public string? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public string? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}

public sealed class OrderDiscount
{
    public decimal Amount { get; set; }

    public string Description { get; set; } = default!;
}

public sealed class OrderVatAmount
{
    public string Name { get; set; } = default!;

    public double Rate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Vat { get; set; }

    public decimal Total { get; set; }
}