using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Events;

namespace YourBrand.Customers.Domain.Entities;

public class Organization : Customer
{
    protected Organization() { }

    public Organization(string name, string organizationNo, string vatNo)
    {
        Name = name;
        OrganizationNo = organizationNo;
        VatNo = vatNo;

        //AddDomainEvent(new PersonCreated(Id));
    }

    public string OrganizationNo { get; set; } = null!;

    public string VatNo { get; set; } = null!;

    public bool? HasCeased { get; set; }
}
