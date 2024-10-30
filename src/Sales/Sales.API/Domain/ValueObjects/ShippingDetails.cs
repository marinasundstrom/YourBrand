
namespace YourBrand.Sales.Domain.ValueObjects;

public record ShippingDetails
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public Address Address { get; set; } = new Address();

    public ShippingDetails? Copy()
    {
        return new ShippingDetails
        {
            FirstName = FirstName,
            LastName = LastName,
            Address = Address.Copy()
        };
    }
}