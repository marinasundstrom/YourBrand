using YourBrand.Catalog.Domain.Common;

namespace YourBrand.Catalog.Domain.Events;

public class AddressCreated : DomainEvent
{
    public AddressCreated(string addressId)
    {
        AddressId = addressId;
    }

    public string AddressId { get; }
}