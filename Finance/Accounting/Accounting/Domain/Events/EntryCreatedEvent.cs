using YourBrand.Accounting.Domain.Common;
using YourBrand.Accounting.Domain.Entities;

namespace YourBrand.Accounting.Domain.Events;

public class EntryCreatedEvent : DomainEvent
{
    public EntryCreatedEvent(Entry entry)
    {
        Entry = entry;
    }

    public Entry Entry { get; }
}