using Core;

using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Sales.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Sales.Domain.Entities;

public class OrderItem : Entity<string>, IAuditableEntity<string, User>, IHasTenant
{
    private readonly HashSet<Discount> _discounts = new HashSet<Discount>();

    internal OrderItem() : base(Guid.NewGuid().ToString()) { }


    public static OrderItem Create(
                       string description,
                       string? productId,
                       decimal price,
                       decimal? regularPrice,
                       double? discountRate,
                       decimal? directDiscount,
                       double quantity,
                       string? unit,
                       bool vatIncluded,
                       double? vatRate,
                       string? notes,
                       TimeProvider timeProvider)
    {
        var orderItem = new OrderItem
        {
            ProductId = productId,
            Description = description,
            Unit = unit,
            Price = price,
            VatIncluded = vatIncluded,
            VatRate = vatRate,
            RegularPrice = regularPrice,
            DiscountRate = discountRate,
            DirectDiscount = directDiscount.GetValueOrDefault(),
            Quantity = quantity,
            Notes = notes
        };

        orderItem.Update(timeProvider);

        return orderItem;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public Order? Order { get; internal set; }

    public string? OrderId { get; internal set; }

    public string Description { get; set; } = null!;

    public string? ProductId { get; set; }

    public string? Sku { get; set; }

    public ProductType ProductType { get; set; }

    public SubscriptionPlan? SubscriptionPlan { get; set; }

    public Guid? SubscriptionPlanId { get; set; }

    public Subscription? Subscription { get; set; }

    public Guid? SubscriptionId { get; set; }

    public string? Unit { get; set; }

    public string? UnitId { get; set; }

    public decimal Price { get; set; }

    public decimal? RegularPrice { get; set; } // Original price before any sales or promotional discounts

    public bool VatIncluded { get; private set; } // Determines if VAT is included in the price

    // Base Price Discount (difference between RegularPrice and Price)
    public decimal BasePriceDiscount => RegularPrice.HasValue && RegularPrice > Price ? (RegularPrice.Value - Price) * (decimal)Quantity : 0;

    // Direct Discount applied directly to the item
    public decimal DirectDiscount { get; private set; } = 0;

    // Promotional Discounts
    private readonly HashSet<Discount> _promotionalDiscounts = new HashSet<Discount>();
    public IReadOnlyCollection<Discount> PromotionalDiscounts => _promotionalDiscounts;

    public decimal PromotionalDiscount => _promotionalDiscounts.Sum(d => d.Amount.GetValueOrDefault() * (decimal)Quantity);

    // Total Discount (sum of all discounts)
    public decimal TotalDiscount => BasePriceDiscount + DirectDiscount + PromotionalDiscount;

    // Methods for managing discounts
    public bool ApplyDirectDiscount(decimal discountAmount, TimeProvider timeProvider)
    {
        DirectDiscount = discountAmount;
        Update(timeProvider); // Recalculate totals
        return true;
    }

    public bool AddPromotionalDiscount(Discount discount, TimeProvider timeProvider)
    {
        if (_promotionalDiscounts.Add(discount))
        {
            Update(timeProvider); // Recalculate totals
            return true;
        }
        return false;
    }

    public bool RemovePromotionalDiscount(Discount discount, TimeProvider timeProvider)
    {
        if (_promotionalDiscounts.Remove(discount))
        {
            Update(timeProvider); // Recalculate totals
            return true;
        }
        return false;
    }

    public double? DiscountRate { get; set; }

    public double Quantity { get; set; }

    public decimal SubTotal { get; private set; }

    public double? VatRate { get; set; }

    public decimal? Vat { get; private set; }

    public decimal Total { get; private set; }

    public string? Notes { get; set; }

    public User? CreatedBy { get; set; }

    public UserId? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    // Update method recalculates totals based on discounts and VAT
    public void Update(TimeProvider timeProvider)
    {
        // Calculate base total after all discounts (before VAT adjustment)
        var baseTotal = (Price * (decimal)Quantity) - TotalDiscount;

        // Calculate VAT and Total based on VAT inclusion status
        CalculateVatAndTotal(baseTotal);
    }

    private void CalculateVatAndTotal(decimal baseTotal)
    {
        if (VatRate.HasValue)
        {
            if (VatIncluded)
            {
                // Extract VAT from baseTotal as VAT is included in Price
                Vat = PriceCalculations.CalculateVat(baseTotal, VatRate.Value);
                SubTotal = baseTotal - Vat.GetValueOrDefault();
                Total = baseTotal;
            }
            else
            {
                Vat = baseTotal * (decimal)VatRate.Value;
                SubTotal = baseTotal;
                Total = baseTotal + Vat.GetValueOrDefault();
            }
        }
        else
        {
            Vat = 0;
            SubTotal = baseTotal;
            Total = baseTotal;
        }
    }

    public void AdjustForVatInclusionChange(bool newVatIncluded, TimeProvider timeProvider)
    {
        if (VatIncluded == newVatIncluded) return; // No change needed if the VAT inclusion status is the same

        VatIncluded = newVatIncluded;

        decimal adjustedPrice = Price;

        if (VatRate.HasValue)
        {
            if (newVatIncluded)
            {
                // VAT was not included, but now needs to be included
                adjustedPrice = Price * (1 + (decimal)VatRate.Value);
            }
            else
            {
                // VAT was included, but now needs to be excluded
                adjustedPrice = Price / (1 + (decimal)VatRate.Value);
            }
        }

        Price = adjustedPrice;
        Update(timeProvider); // Recalculate totals with the adjusted price
    }

    public void CopyTo(OrderItem targetOrderItem)
    {
        var orderItem = this;

        //targetOrderItem.Order = orderItem.Order;
        //targetOrderItem.OrderItem = orderItem;

        targetOrderItem.Description = orderItem.Description;
        targetOrderItem.ProductId = orderItem!.ProductId;
        targetOrderItem.Sku = orderItem!.Sku;
        targetOrderItem.Quantity = orderItem.Quantity;
        targetOrderItem.Unit = orderItem.Unit;
        targetOrderItem.Price = orderItem.Price;
        targetOrderItem.RegularPrice = orderItem.RegularPrice;
        targetOrderItem.VatRate = orderItem.VatRate;
        targetOrderItem.DirectDiscount = orderItem.DirectDiscount;
        targetOrderItem.DiscountRate = orderItem.DiscountRate;
        targetOrderItem.Notes = orderItem?.Notes;

        targetOrderItem.SubscriptionPlan = orderItem!.SubscriptionPlan;
        targetOrderItem.Subscription = orderItem!.Subscription;
    }

    public void CopyToOrder(Order targetOrder, TimeProvider timeProvider)
    {
        var orderItem = this;

        targetOrder.UpdateStatus((int)OrderStatusEnum.Draft, timeProvider);

        if (orderItem.Order?.Customer is not null)
        {
            targetOrder.Customer = new Customer
            {
                Id = orderItem.Order.Customer.Id,
                Name = orderItem.Order.Customer.Name,
                CustomerNo = orderItem.Order.Customer.CustomerNo
            };
        }

        targetOrder.Parent = orderItem.Order;
        targetOrder.Subscription = orderItem.Subscription ?? orderItem.Order.Subscription;
        targetOrder.BillingDetails = orderItem?.Order?.BillingDetails?.Copy(); // orderItem.HasDeliveryDetails ? orderItem?.DeliveryDetails?.Clone() : orderItem?.Order?.DeliveryDetails?.Clone();
        targetOrder.ShippingDetails = orderItem?.Order?.ShippingDetails?.Copy(); // orderItem.HasDeliveryDetails ? orderItem?.DeliveryDetails?.Clone() : orderItem?.Order?.DeliveryDetails?.Clone();
        //delivery.Assignee = orderItem?.Assignee ?? orderItem?.Order?.Assignee;
        targetOrder.Notes = orderItem!.Notes;
    }
}