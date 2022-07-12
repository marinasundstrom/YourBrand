using System;

using YourBrand.Marketing.Domain.Events;

namespace YourBrand.Marketing.Domain.Entities;

public class Prospect
{
    protected Prospect() { }

    public Prospect(string firstName, string lastName, string ssn)
    {
        FirstName = firstName;
        LastName = lastName;
        Ssn = ssn;

        //AddDomainEvent(new ProspectCreated(Id));
    }

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Ssn { get; set; }

    public string Email { get; set; }

    public string? PhoneHome { get; set; }

    public string PhoneMobile { get; set; }
}
