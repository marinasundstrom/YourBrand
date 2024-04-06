using System.ComponentModel.DataAnnotations;

namespace YourBrand.Portal.Addresses;

public class ShippingDetailsFormModel
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Ssn { get; set; }

    public string? CareOf { get; set; }
    
    public string? Organization { get; set; }

    [Required]
    public AddressFormModel Address { get; set; }
}