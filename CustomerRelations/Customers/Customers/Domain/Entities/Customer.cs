using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Enums;

namespace YourBrand.Customers.Domain.Entities;

public abstract class Customer : AggregateRoot<int>, IAuditable
{
    readonly HashSet<Address> _addresses = new HashSet<Address>();

    public Customer() : base()
    {

    }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string PhoneMobile { get; set; } = null!;

    public CustomerType CustomerType => this is Person ? CustomerType.Individual : CustomerType.Organization;

    public IReadOnlyCollection<Address> Addresses => _addresses;

    public void AddAddress(Address address) => _addresses.Add(address);

    public void RemoveAddress(Address address) => _addresses.Remove(address);

    public string? CreatedById { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public string? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}
