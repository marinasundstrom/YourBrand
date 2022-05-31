using YourBrand.Customers.Domain.Common;

namespace YourBrand.Customers.Domain.Events;

public class AddressCreated : DomainEvent
{
    public AddressCreated(string addressId)
    {
        AddressId = addressId;
    }

    public string AddressId { get; }
}