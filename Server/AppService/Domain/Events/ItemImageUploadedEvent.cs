using System;

using Skynet.Domain.Common;

namespace Skynet.Domain.Events;

public class ItemImageUploadedEvent : DomainEvent
{
    public ItemImageUploadedEvent(string id)
    {
        this.Id = id;
    }

    public string Id { get; }
}