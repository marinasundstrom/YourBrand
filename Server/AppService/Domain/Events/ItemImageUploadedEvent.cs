using System;

using Catalog.Domain.Common;

namespace Catalog.Domain.Events;

public class ItemImageUploadedEvent : DomainEvent
{
    public ItemImageUploadedEvent(string id)
    {
        this.Id = id;
    }

    public string Id { get; }
}