using System;

using YourCompany.Domain.Common;

namespace YourCompany.Domain.Events;

public class ItemImageUploadedEvent : DomainEvent
{
    public ItemImageUploadedEvent(string id)
    {
        this.Id = id;
    }

    public string Id { get; }
}