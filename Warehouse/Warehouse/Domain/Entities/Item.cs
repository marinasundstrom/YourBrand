using System;

using YourBrand.Warehouse.Domain.Events;

namespace YourBrand.Warehouse.Domain.Entities;

public class Item
{
    protected Item() { }

    public Item(string firstName, string lastName, string ssn)
    {
        FirstName = firstName;
        LastName = lastName;
        Ssn = ssn;

        //AddDomainEvent(new ItemCreated(Id));
    }

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Ssn { get; set; }

    public string Email { get; set; }

    public string? PhoneHome { get; set; }

    public string PhoneMobile { get; set; }
}
