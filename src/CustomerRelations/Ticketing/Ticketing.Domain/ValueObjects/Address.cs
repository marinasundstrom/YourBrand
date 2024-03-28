using YourBrand.Ticketing.Domain;
using YourBrand.Ticketing.Domain.Entities;
using YourBrand.Ticketing.Domain.Events;

namespace YourBrand.Ticketing.Domain.ValueObjects;

public class Address
{
    // Street
    public string Thoroughfare { get; set; } = null!;

    // Street number
    public string? Premises { get; set; }

    // Suite
    public string? SubPremises { get; set; }

    public string PostalCode { get; set; } = null!;

    // Town or City
    public string Locality { get; set; } = null!;

    // County
    public string SubAdministrativeArea { get; set; } = null!;

    // State
    public string AdministrativeArea { get; set; } = null!;

    public string Country { get; set; } = null!;
}
