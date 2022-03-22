using YourBrand.Domain.Common;

namespace YourBrand.Domain.Events;

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