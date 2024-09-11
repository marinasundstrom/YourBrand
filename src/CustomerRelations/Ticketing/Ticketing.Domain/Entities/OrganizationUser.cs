using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;


namespace YourBrand.Ticketing.Domain.Entities;

public class OrganizationUser : AggregateRoot<string>, IAuditable
{
    public OrganizationUser()
        : base(Guid.NewGuid().ToString())
    {

    }

    public OrganizationUser(string id)
        : base(id)
    {
        Id = id;
    }

    public OrganizationId OrganizationId { get; set; }

    public Organization Organization { get; set; }

    public UserId UserId { get; set; }

    public User User { get; set; }

    public User? CreatedBy { get; set; }

    public UserId? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}