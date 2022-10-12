
using OrderPriceCalculator;

using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Domain.Entities
{
    public class PriceList : AuditableEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidThru { get; set; }

        public int? CustomerNo { get; set; }

        public List<PriceListItem> Items { get; set; } = new List<PriceListItem>();
    }
}