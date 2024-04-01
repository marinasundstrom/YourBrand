namespace YourBrand.Invoicing.Domain.Entities;

public record ShippingDetails
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? CareOf { get; set; }

    public string? Company { get; set; }

    public Address Address { get; set; } = new Address();
}