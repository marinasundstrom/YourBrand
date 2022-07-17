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

        //AddDomainEvent(new PersonCreated(Id));
    }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Ssn { get; set; }

    public bool? IsDeceased { get; set; }
}
