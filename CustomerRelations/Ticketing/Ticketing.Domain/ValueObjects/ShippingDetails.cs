namespace YourBrand.Ticketing.Domain.ValueObjects;

public class ShippingDetails
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? CareOf { get; set; }

    public Address Address { get; set; } = null!;
}