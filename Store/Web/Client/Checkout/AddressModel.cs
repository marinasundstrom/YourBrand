using System.ComponentModel.DataAnnotations;

namespace Client.Checkout;

public class AddressModel
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
    public string? SubAdministrativeArea { get; set; } = null!;

    // State
    public string? AdministrativeArea { get; set; } = null!;

    [Required]
    public string Country { get; set; } = null!;
}

public class ShippingDetailsModel
{
    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    public string? CareOf { get; set; }

    [Required]
    public AddressModel Address { get; set; } = new AddressModel();
}