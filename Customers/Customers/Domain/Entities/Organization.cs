using YourBrand.Customers.Domain.Common;
using YourBrand.Customers.Domain.Events;

namespace YourBrand.Customers.Domain.Entities;

public class Organization : Customer
{
    readonly HashSet<Address> _addresses = new HashSet<Address>();

    protected Organization() { }

    public Organization(string name, string organizationNo, string vatNo)
    {
        Name = name;
        OrganizationNo = organizationNo;
        VatNo = vatNo;

        //AddDomainEvent(new PersonCreated(Id));
    }

    public string OrganizationNo { get; set; }

    public string VatNo { get; set; }

    public bool? HasCeased { get; set; }
}
