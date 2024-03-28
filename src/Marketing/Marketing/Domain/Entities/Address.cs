using YourBrand.Marketing.Domain.Enums;
using YourBrand.Marketing.Domain.Events;

namespace YourBrand.Marketing.Domain.Entities;

public class Address : Entity<string>, IAuditable
{
#nullable disable

    protected Address() : base() { }

#nullable restore

    public Address(AddressType addressType)
    : base(Guid.NewGuid().ToString())
    {
        Type = addressType;

        //AddDomainEvent(new AddressCreated(Id));
    }

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

    public string? CreatedById { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public string? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}
