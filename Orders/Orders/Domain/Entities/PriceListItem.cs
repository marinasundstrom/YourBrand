using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.Entities
{
    public class PriceListItem : AuditableEntity
    {
        public Guid Id { get; set; }

        public string ItemId { get; set; } = null!;

        public decimal Price { get; set; }
    }
}