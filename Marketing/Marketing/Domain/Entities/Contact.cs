using System;

using YourBrand.Marketing.Domain.Common;
using YourBrand.Marketing.Domain.Entities;
using YourBrand.Marketing.Domain.Enums;
using YourBrand.Marketing.Domain.Events;
using YourBrand.Marketing.Domain.ValueObjects;

namespace YourBrand.Marketing.Domain.Entities;

public class Contact : AuditableEntity
{
    private HashSet<Discount> discounts = new HashSet<Discount>();

    protected Contact() { }

    public Contact(string firstName, string lastName, string ssn)
    {
        FirstName = firstName;
        LastName = lastName;
        Ssn = ssn;

        //AddDomainEvent(new ContactCreated(Id));
    }

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public Campaign? Campaign { get; private set; } = null!;

    public ContactStatus Status { get; private set; }

    public int? CustomerId { get; private set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Ssn { get; set; }

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public string? PhoneMobile { get; set; }

    public Domain.ValueObjects.Address? Address { get; set; }

    public IReadOnlyCollection<Discount> Discounts => discounts;

    public void AddDiscount(Discount discount) 
    {
        discounts.Add(discount);
    }
}
