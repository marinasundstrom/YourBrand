using System.ComponentModel.DataAnnotations;

namespace YourBrand.Portal.Addresses;

public class AddressFormModel 
{
    [Required]
    public string Thoroughfare { get; set; }

    [Required]
    public string Premises { get; set; }

    public string? SubPremises { get; set; }

    [Required]
    public string PostalCode { get; set; }

    [Required]
    public string Locality { get; set; }

    public string? SubAdministrativeArea { get; set; }

    [Required]
    public string AdministrativeArea { get; set; }

    [Required]
    public string Country { get; set; }
}
