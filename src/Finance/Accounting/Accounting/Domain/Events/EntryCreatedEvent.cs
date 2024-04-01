using YourBrand.Accounting.Domain.Common;

namespace YourBrand.Accounting.Domain.Events;

public record EntryCreatedEvent : DomainEvent
{
    public EntryCreatedEvent(int entryId)
    {
        EntryId = entryId;
    }

    public int EntryId { get; }
}