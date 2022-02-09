using Catalog.Domain.Common;

namespace Catalog.Domain.Events;

public class ItemCreatedEvent : DomainEvent
{
    public ItemCreatedEvent(string itemId)
    {
        this.ItemId = itemId;
    }

    public string ItemId { get; }
}