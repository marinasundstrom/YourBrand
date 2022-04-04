using System;

using YourBrand.Domain.Common;

namespace YourBrand.Domain.Events;

public class ItemImageUploadedEvent : DomainEvent
{
    public ItemImageUploadedEvent(string id)
    {
        this.Id = id;
    }

    public string Id { get; }
}