namespace YourBrand.Sales.API.Features.OrderManagement.Domain.ValueObjects;

public record BillingDetails
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? SSN { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public Address Address { get; set; } = new Address();

    public BillingDetails? Copy()
    {
        return new BillingDetails
        {
            FirstName = FirstName,
            LastName = LastName,
            SSN = SSN,
            PhoneNumber = PhoneNumber,
            Email = Email,
            Address = Address.Copy()
        };
    }
}