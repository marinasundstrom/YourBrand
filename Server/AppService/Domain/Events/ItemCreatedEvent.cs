using YourCompany.Domain.Common;

namespace YourCompany.Domain.Events;

public class ItemCreatedEvent : DomainEvent
{
    public ItemCreatedEvent(string itemId)
    {
        this.ItemId = itemId;
    }

    public string ItemId { get; }
}