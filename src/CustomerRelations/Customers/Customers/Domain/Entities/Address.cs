using YourBrand.Customers.Domain.Common;
using YourBrand.Customers.Domain.Enums;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Customers.Domain.Entities;

public class Address : AuditableEntity<string>, IHasTenant
{
    public Address() : base(Guid.NewGuid().ToString())
    {
        //AddDomainEvent(new AddressCreated(Id));
    }

    public TenantId TenantId { get; set; }

    public Organization? Organization { get; private set; } = null!;

    public Person? Person { get; private set; } = null!;

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

    public UserId? CreatedById { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}