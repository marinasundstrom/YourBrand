using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Dtos;


// Required Fields
// Optional Fields
public record AddressDto(string Street, string City, string PostalCode, string Country, string? AddressLine2, string? StateOrProvince, string? CareOf)
{
    // Method to return the full address in a formatted way

    // Mapping method from Address to AddressDto
    public static AddressDto FromAddress(Address address)
    {
        return new AddressDto(
            address.Street,
            address.City,
            address.PostalCode,
            address.Country,
            address.AddressLine2,
            address.StateOrProvince,
            address.CareOf
        );
    }

    // Mapping method from AddressDto to Address
    public Address ToAddress()
    {
        return new Address
        {
            Street = Street,
            City = City,
            PostalCode = PostalCode,
            Country = Country,
            AddressLine2 = AddressLine2,
            StateOrProvince = StateOrProvince,
            CareOf = CareOf
        };
    }

    public Address MapOntoAddress(Address address)
    {
        address.Street = Street;
        address.City = City;
        address.PostalCode = PostalCode;
        address.Country = Country;
        address.AddressLine2 = AddressLine2;
        address.StateOrProvince = StateOrProvince;
        address.CareOf = CareOf;

        return address;
    }

    public override string ToString()
    {
        return $"{CareOf}{(CareOf != null ? ", " : "")}{Street}, {AddressLine2}{(AddressLine2 != null ? ", " : "")}{City}, {StateOrProvince}{(StateOrProvince != null ? ", " : "")}{PostalCode}, {Country}";
    }
}