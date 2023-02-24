using YourBrand.Catalog.Domain.Common;

namespace YourBrand.Catalog.Domain.Events;

public record AddressCreated : DomainEvent
{
    public AddressCreated(string addressId)
    {
        AddressId = addressId;
    }

    public string AddressId { get; }
}