using System;

using YourBrand.Domain.Common;

namespace YourBrand.Domain.Events;

public record ItemImageUploadedEvent : DomainEvent
{
    public ItemImageUploadedEvent(string id)
    {
        this.Id = id;
    }

    public string Id { get; }
}