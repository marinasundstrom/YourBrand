using YourBrand.Domain;

namespace YourBrand.Marketing.Domain.Events;

public record AddressCreated : DomainEvent
{
    public AddressCreated(string addressId)
    {
        AddressId = addressId;
    }

    public string AddressId { get; }
}