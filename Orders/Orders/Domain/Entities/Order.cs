using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;

using OrderPriceCalculator;

using YourBrand.Orders.Contracts;
using YourBrand.Orders.Domain.Common;
using YourBrand.Orders.Domain.Enums;
using YourBrand.Orders.Domain.Events;
using YourBrand.Orders.Domain.ValueObjects;

namespace YourBrand.Orders.Domain.Entities
{
    [Index(nameof(OrderNo))]
    public class Order : AuditableEntity, IOrder2WithTotals, IOrder2WithTotalsInternals
    {
        public Guid Id { get; set; }

        public int OrderNo { get; set; }

        public DateTime? OrderDate { get; set; }

        public OrderStatus Status { get; set; } = null!;

        public string StatusId { get; set; } = null!;

        public DateTime StatusDate { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();

        public decimal? SubTotal { get; set; }
        public double? VatRate { get; set; }
        public decimal? Vat { get; set; }

        public decimal? Rounding { get; set; }
        public decimal Total { get; set; }

        public List<OrderTotals> Totals { get; set; } = new List<OrderTotals>();

        public List<OrderCharge> Charges { get; set; } = new List<OrderCharge>();
        public decimal? Charge { get; set; }

        public List<OrderDiscount> Discounts { get; set; } = new List<OrderDiscount>();
        public decimal? Discount { get; set; }

        public string? Note { get; set; }

        public DeliveryDetails? DeliveryDetails { get; set; }
        public BillingDetails? BillingDetails { get; set; }

        public Subscription? Subscription { get; set; }
        public Guid? SubscriptionId { get; set; }

        public DateTime? PlannedStartDate { get; set; }
        public DateTime? PlannedEndDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ActualEndDate { get; set; }

        public List<CustomField> CustomFields { get; set; } = new List<CustomField>();

        public Order SetPlannedDates(DateTime start, DateTime? end)
        {
            this.PlannedStartDate = start;
            this.PlannedEndDate = end;

            return this;
        }

        /*
        public void AddItem(string itemId, double quantity)
        {
            Items.Add(new OrderItem(itemId, quantity));
        }
        */

        public void DeleteItem(OrderItem item)
        {
            Items.Remove(item);
        }

        public void UpdateOrderStatus(string? orderStatusId)
        {
            StatusId = orderStatusId!;
            StatusDate = DateTime.Now;

            //AddDomainEvent(new OrderStatusChangedEvent(Id, orderStatusId));

            if (orderStatusId == default)
            {
                //AddDomainEvent(new OrderCancelledEvent(Id));
            }
        }

        public void Clear()
        {
            foreach (var item in Items)
            {
                item.Charges.Clear();
            }

            foreach (var item in Items)
            {
                item.Discounts.Clear();
            }

            Items.Clear();
            Charges.Clear();
            Discounts.Clear();

            AddDomainEvent(new OrderClearedEvent(OrderNo));
        }

        public void Place()
        {
            UpdateOrderStatus(default);
        }

        public void Cancel()
        {
            UpdateOrderStatus(default);
        }

        public void Delete()
        {
            //AddDomainEvent(new OrderDeletedEvent(Id));
        }

        public DateTime? Deleted { get; set; }

        public string? DeletedById { get; set; }

        IEnumerable<IOrderItem> IOrder.Items => Items;
        IEnumerable<IOrderItem2> IOrder2.Items => Items;

        IEnumerable<IOrderTotals> IOrder2WithTotals.Totals => Totals;

        IEnumerable<ICharge> IHasCharges.Charges => Charges;
        IEnumerable<IChargeWithTotal> IHasChargesWithTotal.Charges => Charges;

        IEnumerable<IDiscount> IHasDiscounts.Discounts => Discounts;
        IEnumerable<IDiscountWithTotal> IHasDiscountsWithTotal.Discounts => Discounts;

        void IOrder2WithTotalsInternals.AddTotals(IOrderTotals orderTotals)
        {
            Totals.Add((OrderTotals)orderTotals);
        }

        void IOrder2WithTotalsInternals.RemoveTotals(IOrderTotals orderTotals)
        {
            Totals.Remove((OrderTotals)orderTotals);
        }

        void IOrder2WithTotalsInternals.ClearTotals()
        {
            Totals.Clear();
        }

        IOrderTotals IOrder2WithTotalsInternals.CreateTotals(double vatRate, decimal subTotal, decimal vat, decimal total)
        {
            return new OrderTotals()
            {
                VatRate = vatRate,
                SubTotal = subTotal,
                Vat = vat,
                Total = total
            };
        }
    }

    public class OrderStatus : AuditableEntity, ISoftDelete
    {
        public OrderStatus()
        {

        }

        public OrderStatus(string id, string name, string? description = null)
        {
            Id = id;
            Name = name;
            Description = description;
        }

        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? Deleted { get; set; }

        public string? DeletedById { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class OrderTotals : AuditableEntity, IOrderTotals
    {
        public Guid Id { get; set; }
        public Order Order { get; set; } = null!;
        public double VatRate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public decimal Total { get; set; }
    }

    public class OrderItem : AuditableEntity, IOrderItem2, ISoftDelete
    {
        public Guid Id { get; set; }

        public Order Order { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string ItemId { get; set; } = null!;

        public string? Unit { get; set; }
        public decimal Price { get; set; }
        public double VatRate { get; set; }
        public double Quantity { get; set; }
        public decimal Vat { get; set; }
        public decimal Total { get; set; }

        public List<OrderCharge> Charges { get; set; } = new List<OrderCharge>();

        public decimal? Charge { get; set; }
        public List<OrderDiscount> Discounts { get; set; } = new List<OrderDiscount>();
        public decimal? Discount { get; set; }

        public string? Note { get; set; }

        public bool HasDeliveryDetails { get; set; }
        public DeliveryDetails? DeliveryDetails { get; set; }

        public Subscription? Subscription { get; set; }
        public Guid? SubscriptionId { get; set; }

        public List<CustomField> CustomFields { get; set; } = new List<CustomField>();


        IEnumerable<ICharge> IHasCharges.Charges => Charges;

        IEnumerable<IChargeWithTotal> IHasChargesWithTotal.Charges => Charges;

        IEnumerable<IDiscount> IHasDiscounts.Discounts => Discounts;
        IEnumerable<IDiscountWithTotal> IHasDiscountsWithTotal.Discounts => Discounts;

        public OrderItem UpdateQuantity(double quantity)
        {
            Quantity = quantity;

            return this;
        }

        public void Clear()
        {
            Charges.Clear();
            Discounts.Clear();
        }

        public DateTime? Deleted { get; set; }

        public string? DeletedById { get; set; }
    }

    public class OrderCharge : IChargeWithTotal
    {
        public Guid Id { get; set; }

        public Order? Order { get; set; } = null!;

        public OrderItem? OrderItem { get; set; }

        public string Description { get; set; } = null!;

        public Guid? ChargeId { get; set; }

        public int? Quantity { get; set; }

        public int? Limit { get; set; }

        public decimal? Amount { get; set; }

        public double? Percent { get; set; }

        public decimal Total { get; set; }
    }

    public class OrderDiscount : AuditableEntity, IDiscountWithTotal
    {
        public Guid Id { get; set; }

        public Order? Order { get; set; } = null!;

        public OrderItem? OrderItem { get; set; }

        public string Description { get; set; } = null!;

        public Guid? DiscountId { get; set; }

        public int? Quantity { get; set; }

        public int? Limit { get; set; }

        public decimal? Amount { get; set; }

        public double? Percent { get; set; }

        public decimal Total { get; set; }
    }
}