using YourBrand.Marketing.Domain.Common;

namespace YourBrand.Marketing.Domain.Events;

public class AddressCreated : DomainEvent
{
    public AddressCreated(string addressId)
    {
        AddressId = addressId;
    }

    public string AddressId { get; }
}