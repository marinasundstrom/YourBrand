using YourBrand.Catalog.Domain.Common;

namespace YourBrand.Catalog.Domain.Events;

public class PersonCreated : DomainEvent
{
    public PersonCreated(string personId)
    {
        PersonId = personId;
    }

    public string PersonId { get; }
}
