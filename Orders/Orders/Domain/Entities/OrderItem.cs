using OrderPriceCalculator;

using YourBrand.Orders.Domain.Common;
using YourBrand.Orders.Domain.ValueObjects;

namespace YourBrand.Orders.Domain.Entities
{
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
}