using YourBrand.Warehouse.Domain.Common;

namespace YourBrand.Warehouse.Domain.Events;

public class ItemCreated : DomainEvent
{
    public ItemCreated(string personId)
    {
        ItemId = personId;
    }

    public string ItemId { get; }
}
