using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Meetings.Domain.Entities;

public class Organization : AggregateRoot<OrganizationId>, IOrganization, IAuditableEntity<OrganizationId>, IHasTenant
{
    public Organization(OrganizationId id, string name)
        : base(id)
    {
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