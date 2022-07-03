using YourBrand.Products.Domain.Common;

namespace YourBrand.Products.Domain.Events;

public class PersonCreated : DomainEvent
{
    public PersonCreated(string personId)
    {
        PersonId = personId;
    }

    public string PersonId { get; }
}
