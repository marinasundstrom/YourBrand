using YourBrand.Accounting.Domain.Common;
using YourBrand.Accounting.Domain.Entities;

namespace YourBrand.Accounting.Domain.Events;

public record EntryCreatedEvent : DomainEvent
{
    public EntryCreatedEvent(int entryId)
    {
        EntryId = entryId;
    }

    public int EntryId { get; }
}