namespace YourBrand.Sales.API.Features.OrderManagement.Domain.ValueObjects;

public record ShippingDetails
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? CareOf { get; set; }
    public Address Address { get; set; } = new Address();
}