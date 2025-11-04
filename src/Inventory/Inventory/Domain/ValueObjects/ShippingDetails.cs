using YourBrand.Domain;

namespace YourBrand.Inventory.Domain.ValueObjects;

public class ShippingDetails
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? CareOf { get; set; }

    public Address Address { get; set; } = new();

    public ShippingDetails Copy()
    {
        return new ShippingDetails
        {
            FirstName = FirstName,
            LastName = LastName,
            CareOf = CareOf,
            Address = Address.Copy()
        };
    }
}
