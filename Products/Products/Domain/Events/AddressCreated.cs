using YourBrand.Products.Domain.Common;

namespace YourBrand.Products.Domain.Events;

public class AddressCreated : DomainEvent
{
    public AddressCreated(string addressId)
    {
        AddressId = addressId;
    }

    public string AddressId { get; }
}