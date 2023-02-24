using YourBrand.Marketing.Domain.Common;

namespace YourBrand.Marketing.Domain.Events;

public record ContactCreated : DomainEvent
{
    public ContactCreated(string personId)
    {
        ContactId = personId;
    }

    public string ContactId { get; }
}
