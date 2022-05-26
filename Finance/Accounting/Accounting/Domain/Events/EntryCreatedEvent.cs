using Accounting.Domain.Common;
using Accounting.Domain.Entities;

namespace Accounting.Domain.Events;

public class EntryCreatedEvent : DomainEvent
{
    public EntryCreatedEvent(Entry entry)
    {
        Entry = entry;
    }

    public Entry Entry { get; }
}