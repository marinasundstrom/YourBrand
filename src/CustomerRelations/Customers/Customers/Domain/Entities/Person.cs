using System;

using YourBrand.Customers.Domain.Events;

namespace YourBrand.Customers.Domain.Entities;

public class Person : Customer
{
    protected Person() { }

    public Person(string firstName, string lastName, string ssn)
    {
        FirstName = firstName;
        LastName = lastName;
        Ssn = ssn;

        Name = $"{FirstName} {LastName}";

        //AddDomainEvent(new PersonCreated(Id));
    }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Ssn { get; set; } = null!;

    public bool? IsDeceased { get; set; }
}
