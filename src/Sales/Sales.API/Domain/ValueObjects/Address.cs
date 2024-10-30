namespace YourBrand.Sales.Domain.ValueObjects;

public record Address
{
    // Required Fields
    public string Street { get; set; } // e.g., "123 Main St"
    public string City { get; set; } // e.g., "Springfield"
    public string PostalCode { get; set; } // e.g., "12345" (can accommodate postal codes with letters and hyphens)
    public string Country { get; set; } // e.g., "USA", "SE" (ISO country code or full country name)

    // Optional Fields
    public string? AddressLine2 { get; set; } // e.g., "Apt 4B" or additional address details
    public string? StateOrProvince { get; set; } // e.g., "CA" or "Stockholm"
    public string? CareOf { get; set; } // e.g., "C/O John Doe" for care-of situations

    // Method to copy an address object
    public Address Copy()
    {
        return new Address
        {
            Street = this.Street,
            City = this.City,
            PostalCode = this.PostalCode,
            Country = this.Country,
            AddressLine2 = this.AddressLine2,
            StateOrProvince = this.StateOrProvince,
            CareOf = this.CareOf
        };
    }

    // Method to return the full address in a formatted way
    public override string ToString()
    {
        return $"{CareOf}{(CareOf != null ? ", " : "")}{Street}, {AddressLine2}{(AddressLine2 != null ? ", " : "")}{City}, {StateOrProvince}{(StateOrProvince != null ? ", " : "")}{PostalCode}, {Country}";
    }
}