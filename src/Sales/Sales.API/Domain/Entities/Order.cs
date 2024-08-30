using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Sales.Domain.Events;
using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Domain.Entities;

public class Order : AggregateRoot<string>, IAuditable, IHasTenant, IHasOrganization
{
    readonly HashSet<OrderItem> _items = new HashSet<OrderItem>();

    private Order() : base(Guid.NewGuid().ToString())
    {
        StatusId = 1;
    }

    public static Order Create(OrganizationId organizationId)
    {
        return new Order()
        {
            OrganizationId = organizationId
        };
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int OrderNo { get; set; }

    public DateTime Date { get; private set; } = DateTime.Now;

    public OrderStatus Status { get; set; } = null!;

    public int StatusId { get; set; }

    public DateTimeOffset? StatusDate { get; set; }

    public bool UpdateStatus(int status)
    {
        var oldStatus = StatusId;
        if (status != oldStatus)
        {
            StatusId = status;
            StatusDate = DateTimeOffset.UtcNow;

            AddDomainEvent(new OrderUpdated(Id));
            AddDomainEvent(new OrderStatusUpdated(Id, status, oldStatus));

            return true;
        }

        return false;
    }
    
    public Order? Parent { get; set; }

    public User? Assignee { get; private set; }

    public UserId? AssigneeId { get; private set; }

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

    public Subscription? Subscription { get; set; }
    public Guid? SubscriptionId { get; set; }

    public DateTime? PlannedStartDate { get; set; }
    public DateTime? PlannedEndDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public DateTime? ActualEndDate { get; set; }

    public Order SetPlannedDates(DateTime start, DateTime? end)
    {
        this.PlannedStartDate = start;
        this.PlannedEndDate = end;

        return this;
    }

    public Customer? Customer { get; set; }

    public bool VatIncluded { get; set; }

    public string Currency { get; set; } = "SEK";

    public string? Reference { get; private set; }

    public string? Notes { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Discount { get; set; }

    public List<OrderDiscount> Discounts { get; set; } = new List<OrderDiscount>();

    public double? VatRate { get; set; }

    public decimal? Vat { get; set; }

    public List<OrderVatAmount> VatAmounts { get; set; } = new List<OrderVatAmount>();

    public bool Rounding { get; set; }

    public decimal? Rounded { get; set; }

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
        orderItem.OrganizationId = OrganizationId;
        orderItem.Order = this;
        _items.Add(orderItem);

        Update();

        return orderItem;
    }

    public OrderItem AddItem(OrderItem orderItem)
    {
        _items.Add(orderItem);

        Update();

        return orderItem;
    }

    public void RemoveOrderItem(OrderItem orderItem) => _items.Remove(orderItem);

    public void Update()
    {
        UpdateVatAmounts();

        Vat = Items.Sum(x => x.Vat.GetValueOrDefault());
        Total = Items.Sum(x => x.Total);
        SubTotal = VatIncluded ? (Total - Vat.GetValueOrDefault()) : Total;
        Discount = Items.Sum(x => (decimal)x.Quantity * x.Discount.GetValueOrDefault());

        Rounded = null;
        if (Rounding)
        {
            Rounded = Math.Round(0m, MidpointRounding.AwayFromZero);
        }
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

            var vatAmount = VatAmounts.FirstOrDefault(x => x.VatRate == item.VatRate);
            if (vatAmount is null)
            {
                vatAmount = new OrderVatAmount()
                {
                    VatRate = item.VatRate.GetValueOrDefault(),
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
            if (x.Vat == 0 && x.VatRate != 0)
            {
                VatAmounts.Remove(x);
            }
        });

        if (VatAmounts.Count == 1)
        {
            var vatAmount = VatAmounts.First();

            VatRate = vatAmount.VatRate;
        }
        else
        {
            VatRate = null;
        }
    }

    public User? CreatedBy { get; set; }

    public UserId? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}

[Index(nameof(Id), nameof(CustomerNo))]
public class Customer
{
    public string Id { get; set; }

    public long CustomerNo { get; set; }

    public string Name { get; set; }
}

public sealed class OrderDiscount
{
    public decimal Amount { get; set; }

    public string Description { get; set; } = default!;
}

public sealed class OrderVatAmount
{
    public string Name { get; set; } = default!;

    public double VatRate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Vat { get; set; }

    public decimal Total { get; set; }
}