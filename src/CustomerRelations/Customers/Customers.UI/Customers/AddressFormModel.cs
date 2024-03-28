using System.ComponentModel.DataAnnotations;

namespace YourBrand.Customers.Customers;

public class AddressFormModel
{
    // Street
    [Required]
    public string Thoroughfare { get; set; } = null!;

    // Street number
    [Required]
    public string? Premises { get; set; }

    // Suite
    public string? SubPremises { get; set; }

    [Required]
    public string PostalCode { get; set; } = null!;

    // Town or City
    [Required]
    public string Locality { get; set; } = null!;

    // County
    [Required]
    public string SubAdministrativeArea { get; set; } = null!;

    // State
    [Required]
    public string AdministrativeArea { get; set; } = null!;

    [Required]
    public string Country { get; set; } = null!;
}