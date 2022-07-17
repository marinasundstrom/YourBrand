using YourBrand.Marketing.Domain.Common;
using YourBrand.Marketing.Domain.Enums;
using YourBrand.Marketing.Domain.Events;

namespace YourBrand.Marketing.Domain.Entities;

public class Address : AuditableEntity
{
    public Address()
    {
        Id = Guid.NewGuid().ToString();

        //AddDomainEvent(new AddressCreated(Id));
    }

    public string Id { get; private set; }

    public AddressType Type { get; set; }

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
