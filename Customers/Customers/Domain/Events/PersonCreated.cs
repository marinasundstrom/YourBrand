using YourBrand.Customers.Domain.Common;

namespace YourBrand.Customers.Domain.Events;

public record PersonCreated : DomainEvent
{
    public PersonCreated(string personId)
    {
        PersonId = personId;
    }

    public string PersonId { get; }
}
