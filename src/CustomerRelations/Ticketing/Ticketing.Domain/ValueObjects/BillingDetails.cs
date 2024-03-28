namespace YourBrand.Ticketing.Domain.ValueObjects;

public class BillingDetails
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? SSN { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public Address Address { get; set; } = null!;
}
