namespace YourBrand.Invoicing.Domain.Entities;

public record BillingDetails
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Company { get; set; }
    public string? SSN { get; set; }
    public string? OrgNo { get; set; }
    public string? VatNo { get; set; }
    public string PhoneNumber { get; set; }
    public string Email { get; set; }
    public Address Address { get; set; } = new Address();
}
