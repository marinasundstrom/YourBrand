using YourBrand.Customers.Domain.Common;

namespace YourBrand.Customers.Domain.Events;

public record AddressCreated : DomainEvent
{
    public AddressCreated(string addressId)
    {
        AddressId = addressId;
    }

    public string AddressId { get; }
}