using System.ComponentModel.DataAnnotations;

namespace YourBrand.Portal.Addresses;

public class BillingDetailsFormModel
{
    public string? Organization { get; set; }

    public string? VatNumber { get; set; }

    [Required]
    public string FirstName { get; set; }

     [Required]
    public string LastName { get; set; }

    [Required]
    public string Ssn { get; set; }

    [Required]
    public string PhoneNumber { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public AddressFormModel Address { get; set; }
}
