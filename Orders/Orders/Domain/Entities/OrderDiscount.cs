using OrderPriceCalculator;

using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.Entities
{
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