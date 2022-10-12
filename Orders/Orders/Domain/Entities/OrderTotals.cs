using OrderPriceCalculator;

using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.Entities
{
    public class OrderTotals : AuditableEntity, IOrderTotals
    {
        public Guid Id { get; set; }
        public Order Order { get; set; } = null!;
        public double VatRate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public decimal Total { get; set; }
    }
}