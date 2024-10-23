using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.Ticketing.Domain.ValueObjects;

namespace YourBrand.Ticketing.Domain.Entities;

public class Organization : AggregateRoot<OrganizationId>, IOrganization, IAuditable, IHasTenant
{
    public Organization(OrganizationId id, string name)
        : base(id)
    {
        Id = id;
        Name = name;
    }

    public TenantId TenantId { get; set; }

    public string Name { get; set; }

    public User? CreatedBy { get; set; }

    public UserId? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    public List<User> Users { get; set; }

    public List<OrganizationUser> OrganizationUsers { get; set; }

    public bool HasUser(UserId userId)
    {
        return Users.Any(x => x.Id == userId);
    }
}