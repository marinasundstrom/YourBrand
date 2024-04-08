using YourBrand.Identity;
using YourBrand.Marketing.Domain.Enums;

namespace YourBrand.Marketing.Domain.Entities;

public class Contact : Entity<string>, IAuditable
{
    private readonly HashSet<Discount> discounts = new HashSet<Discount>();

#nullable disable

    protected Contact() : base() { }

#nullable restore

    public Contact(string firstName, string lastName, string ssn)
    : base(Guid.NewGuid().ToString())
    {
        FirstName = firstName;
        LastName = lastName;
        Ssn = ssn;

        //AddDomainEvent(new ContactCreated(Id));
    }

    public Campaign? Campaign { get; set; } = null!;

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

    public UserId? CreatedById { get; set; } = null!;

    public DateTimeOffset Created { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}