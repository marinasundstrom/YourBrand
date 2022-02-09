using Catalog.Domain.Common;

namespace Catalog.Domain.Events;

public class ItemDeletedEvent : DomainEvent
{
    public ItemDeletedEvent(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public string Id { get; }

    public string Name { get; }
}