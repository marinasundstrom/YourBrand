using YourBrand.Customers.Domain.Common;

namespace YourBrand.Customers.Domain.Events;

public class PersonCreated : DomainEvent
{
    public PersonCreated(string personId)
    {
        PersonId = personId;
    }

    public string PersonId { get; }
}