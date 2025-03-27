
using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Sales.Domain.Events;
using YourBrand.Sales.Domain.ValueObjects;
using YourBrand.Sales.Features.OrderManagement.Orders;

namespace YourBrand.Sales.Domain.Entities;

public class Order : AggregateRoot<string>, IAuditableEntity<string, User>, IHasTenant, IHasOrganization
{
    readonly HashSet<OrderItem> _items = new HashSet<OrderItem>();

    readonly HashSet<Discount> _discounts = new HashSet<Discount>();

    internal Order() : base(Guid.NewGuid().ToString())
    {
        StatusId = (int)OrderStatusEnum.Draft;
        TypeId = 1;
    }

    public Order(
        OrganizationId organizationId,
        int? typeId,
        bool vatIncluded,
        string currency = "SEK",
        int initialStatus = (int)OrderStatusEnum.Draft,
        Customer? customer = null,
        BillingDetails? billingDetails = null,
        ShippingDetails? shippingDetails = null,
        Subscription? subscription = null,
        OrderSchedule? schedule = null) : this()
    {
        StatusId = initialStatus;
        TypeId = typeId ?? 1;
        OrganizationId = organizationId;
        VatIncluded = vatIncluded;
        Currency = currency;
        InitialStatus = initialStatus;
        Customer = customer;
        BillingDetails = billingDetails;
        ShippingDetails = shippingDetails;
        Subscription = subscription;
        Schedule = schedule;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int? OrderNo { get; private set; }

    public async Task AssignOrderNo(OrderNumberFetcher orderNumberFetcher, CancellationToken cancellationToken = default)
    {
        if (OrderNo is not null)
        {
            throw new InvalidOperationException("Order number already set");
        }

        OrderNo = await orderNumberFetcher.GetNextNumberAsync(OrganizationId, cancellationToken);
    }

    public void AssignOrderNo(int orderNo)
    {
        if (OrderNo is not null)
        {
            throw new InvalidOperationException("Order number already set");
        }

        OrderNo = orderNo;
    }

    public DateTimeOffset? Date { get; private set; }

    public OrderType Type { get; set; } = null!;
    public int TypeId { get; set; }

    public OrderStatus Status { get; private set; } = null!;
    public int StatusId { get; private set; } = 1;

    public DateTimeOffset? StatusDate { get; private set; }

    public bool UpdateStatus(int status, TimeProvider timeProvider)
    {
        var oldStatus = StatusId;
        if (status != oldStatus)
        {
            if (oldStatus == 1)
            {
                Date = timeProvider.GetUtcNow();
            }

            StatusId = status;
            StatusDate = timeProvider.GetUtcNow();

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

    public Customer? Customer { get; set; }

    public OrderSchedule? Schedule { get; private set; }

    public Order AssignSchedule(Action<OrderSchedule> config)
    {
        if (Schedule is not null)
        {
            throw new InvalidOperationException("A Schedule has already been assigned. Modify it through the Schedule property.");
        }
        var schedule = new OrderSchedule();
        config(schedule);
        Schedule = schedule;
        return this;
    }

    public bool VatIncluded { get; private set; }

    public void SetVatIncluded(bool vatIncluded, TimeProvider timeProvider)
    {
        if (VatIncluded == vatIncluded) return; // No change needed if the value is the same

        // Update the VatIncluded setting
        VatIncluded = vatIncluded;

        // Adjust all items based on the new VatIncluded setting
        foreach (var item in _items)
        {
            item.AdjustForVatInclusionChange(VatIncluded, timeProvider);
        }

        // Recalculate order totals to reflect the changes
        Update(timeProvider);
    }

    public string Currency { get; set; } = "SEK";

    // New property to determine if discounts should apply before or after VAT
    public bool ApplyDiscountBeforeVat { get; set; } = true;

    public int InitialStatus { get; }
    public string? Reference { get; private set; }

    public string? Notes { get; set; }

    public decimal SubTotal { get; set; }

    public IReadOnlyCollection<Discount> Discounts => _discounts;

    public bool AddDiscount(string description, decimal amount, string? discountId = null, TimeProvider timeProvider)
    {
        var discount = new Discount { OrganizationId = OrganizationId, Description = description, Amount = amount, DiscountId = discountId };
        if (_discounts.Contains(discount)) return false;
        _discounts.Add(discount);
        Update(timeProvider);
        return true;
    }

    public bool RemoveDiscount(Discount discount, TimeProvider timeProvider)
    {
        var removed = _discounts.Remove(discount);
        if (removed) Update(timeProvider);
        return removed;
    }

    public decimal Discount { get; set; }

    public double? VatRate { get; set; }

    public decimal? Vat { get; set; }

    public List<OrderVatAmount> VatAmounts { get; set; } = new List<OrderVatAmount>();

    public bool Rounding { get; set; }

    public decimal? Rounded { get; set; }

    public decimal Total { get; set; }

    public decimal TotalDiscount { get; set; }

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
                       string? notes,
                       TimeProvider timeProvider)
    {
        // Adjust price based on the Order's VatIncluded setting
        //decimal adjustedPrice = AdjustItemPriceForVat(price, vatRate);

        var orderItem = OrderItem.Create(
            description, itemId!, price, regularPrice, discountRate, discount, quantity, unit, VatIncluded, vatRate, notes,
            timeProvider);

        return AddItem(orderItem, timeProvider);
    }

    public OrderItem AddItem(OrderItem orderItem, TimeProvider timeProvider)
    {
        orderItem.OrganizationId = OrganizationId;
        orderItem.Order = this;
        _items.Add(orderItem);

        Update(timeProvider);

        return orderItem;
    }

    public void RemoveOrderItem(OrderItem orderItem) => _items.Remove(orderItem);

    public void Update(TimeProvider timeProvider)
    {
        ApplyDiscounts(timeProvider);
        ApplyVat(timeProvider);
        ApplyRounding();

        TotalDiscount = CalculateTotalDiscount(timeProvider);
    }

    private void ApplyDiscounts(TimeProvider timeProvider)
    {
        decimal totalOrderDiscount = 0m;
        foreach (var discount in _discounts)
        {
            if (discount.IsValid(timeProvider))
            {
                totalOrderDiscount += discount.ApplyTo(this);
            }
        }
        Discount = totalOrderDiscount;
    }

    private void ApplyVat(TimeProvider timeProvider)
    {
        UpdateVatAmounts(timeProvider);

        Vat = Items.Sum(x => x.Vat.GetValueOrDefault());
        Total = Items.Sum(x => x.Total) - Discount;
        SubTotal = VatIncluded ? (Total - Vat.GetValueOrDefault()) : Total;
    }

    private void ApplyRounding()
    {
        Rounded = null;
        if (Rounding)
        {
            Rounded = Math.Round(Total, MidpointRounding.AwayFromZero);
        }
    }

    private void UpdateVatAmounts(TimeProvider timeProvider)
    {
        VatAmounts.ForEach(x =>
        {
            x.Vat = 0;
            x.SubTotal = 0;
            x.Total = 0;
        });

        foreach (var item in Items)
        {
            item.Update(timeProvider);

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

    private decimal CalculateTotalDiscount(TimeProvider timeProvider)
    {
        // Calculate total discounts applied to items
        var itemsDiscount = Items.Sum(x => x.TotalDiscount);

        // Calculate total discounts at the order level
        decimal orderDiscount = 0m;
        foreach (var discount in _discounts)
        {
            if (discount.IsValid(timeProvider)) // Ensure discount is valid
            {
                orderDiscount += discount.ApplyTo(this);
            }
        }

        // Sum item-level and order-level discounts
        return itemsDiscount + orderDiscount;
    }

    public void Complete(TimeProvider timeProvider)
    {
        UpdateStatus((int)OrderStatusEnum.Completed, timeProvider);
    }

    public User? CreatedBy { get; set; }

    public UserId? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    public void CopyTo(Order targetOrder, TimeProvider timeProvider,
        bool copySubscription = false, bool makeChild = false)
    {
        var order = this;

        targetOrder.VatIncluded = order.VatIncluded;

        targetOrder.UpdateStatus((int)OrderStatusEnum.Draft, timeProvider);

        if (order?.Customer is not null)
        {
            targetOrder.Customer = new Customer
            {
                Id = order.Customer.Id,
                Name = order.Customer.Name,
                CustomerNo = order.Customer.CustomerNo
            };
        }

        if (makeChild)
        {
            targetOrder.Parent = order;
        }

        if (copySubscription)
        {
            targetOrder.Subscription = this.Subscription;
        }

        targetOrder.Notes = order?.Notes;
        targetOrder.BillingDetails = order?.BillingDetails?.Copy();
        targetOrder.ShippingDetails = order?.ShippingDetails?.Copy();
        //delivery.Assignee = order?.Assignee;
    }
}

public class OrderSchedule
{
    public DateTimeOffset? PlannedStartDate { get; private set; }
    public DateTimeOffset? PlannedEndDate { get; private set; }
    public DateTimeOffset? ActualStartDate { get; private set; }
    public DateTimeOffset? ActualEndDate { get; private set; }

    public OrderSchedule(DateTimeOffset? plannedStartDate = null, DateTimeOffset? plannedEndDate = null)
    {
        if (plannedEndDate.HasValue && plannedStartDate.HasValue && plannedEndDate < plannedStartDate)
        {
            throw new ArgumentException("Planned end date cannot be earlier than planned start date.");
        }

        PlannedStartDate = plannedStartDate;
        PlannedEndDate = plannedEndDate;
    }

    public void SetPlannedStartDate(DateTimeOffset? startDate)
    {
        if (PlannedEndDate.HasValue && startDate.HasValue && startDate > PlannedEndDate)
        {
            throw new ArgumentException("Planned start date cannot be after the planned end date.");
        }

        PlannedStartDate = startDate;
    }

    public void SetPlannedEndDate(DateTimeOffset? endDate)
    {
        if (PlannedStartDate.HasValue && endDate.HasValue && endDate < PlannedStartDate)
        {
            throw new ArgumentException("Planned end date cannot be before the planned start date.");
        }

        PlannedEndDate = endDate;
    }

    public void SetActualStartDate(DateTimeOffset? startDate)
    {
        if (PlannedStartDate.HasValue && startDate.HasValue && startDate < PlannedStartDate)
        {
            throw new ArgumentException("Actual start date cannot be before the planned start date.");
        }

        if (PlannedEndDate.HasValue && startDate.HasValue && startDate > PlannedEndDate)
        {
            throw new ArgumentException("Actual start date cannot be after the planned end date.");
        }

        ActualStartDate = startDate;
    }

    public void SetActualEndDate(DateTimeOffset? endDate)
    {
        if (ActualStartDate.HasValue && endDate.HasValue && endDate < ActualStartDate)
        {
            throw new ArgumentException("Actual end date cannot be before the actual start date.");
        }

        if (PlannedEndDate.HasValue && endDate.HasValue && endDate > PlannedEndDate)
        {
            throw new ArgumentException("Actual end date cannot be after the planned end date.");
        }

        ActualEndDate = endDate;
    }

    public bool IsScheduled => PlannedStartDate.HasValue && PlannedEndDate.HasValue;
    public bool IsStarted => ActualStartDate.HasValue;
    public bool IsCompleted => ActualEndDate.HasValue;
}

[Index(nameof(Id), nameof(CustomerNo))]
public class Customer
{
    public string Id { get; set; }

    public long CustomerNo { get; set; }

    public string Name { get; set; }
}

public sealed class OrderVatAmount
{
    public string Name { get; set; } = default!;

    public double VatRate { get; set; }

    public decimal SubTotal { get; set; }

    public decimal Vat { get; set; }

    public decimal Total { get; set; }
}

public enum OrderStatusEnum
{
    Draft = 1,
    Planned,
    PendingConfirmation,
    Confirmed,
    PaymentProcessing,
    PaymentFailed,
    Processing,
    Shipped,
    InTransit,
    OutForDelivery,
    Delivered,
    Completed,
    Canceled,
    OnHold,
    Returned,
    Refunded
}