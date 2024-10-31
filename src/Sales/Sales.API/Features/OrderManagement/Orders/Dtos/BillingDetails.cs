using YourBrand.Domain;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

public class BillingDetailsDto
{
    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? SSN { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Email { get; set; } = null!;

    public AddressDto Address { get; set; } = null!;
}