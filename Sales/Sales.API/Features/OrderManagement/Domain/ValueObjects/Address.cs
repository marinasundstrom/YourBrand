using YourBrand.Sales.API.Features.OrderManagement.Domain;
using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;
using YourBrand.Sales.API.Features.OrderManagement.Domain.Events;

namespace YourBrand.Sales.API.Features.OrderManagement.Domain.ValueObjects;

public record Address
{
    // Street
    public string Thoroughfare { get; set; }

    // Street number
    public string Premises { get; set; }

    // Suite
    public string? SubPremises { get; set; }

    public string PostalCode { get; set; }

    // Town or City
    public string Locality { get; set; }

    // County or Municipality
    public string SubAdministrativeArea { get; set; }

    // State or Province
    public string AdministrativeArea { get; set; }

    public string Country { get; set; }
}