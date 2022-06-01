using System;

using YourBrand.Customers.Domain.Common;
using YourBrand.Customers.Domain.Events;

namespace YourBrand.Customers.Domain.Entities;

public class Person : AuditableEntity, IHasDomainEvents
{
    readonly HashSet<Address> _addresses = new HashSet<Address>();

    protected Person() { }

    public Person(string firstName, string lastName, string ssn)
    {
        Id = Guid.NewGuid().ToString();
        FirstName = firstName;
        LastName = lastName;
        Ssn = ssn;

        DomainEvents.Add(new PersonCreated(Id));
    }

    public string Id { get; private set; } 

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Ssn { get; set; }

    public string Email { get; set; }

    public string? PhoneHome { get; set; }

    public string PhoneMobile { get; set; }

    public IReadOnlyCollection<Address> Addresses => _addresses;

    public void AddAddress(Address address) => _addresses.Add(address);

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();
}
