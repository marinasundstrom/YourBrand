using YourBrand.Meetings.Domain.ValueObjects;
using YourBrand.Tenancy;
using YourBrand.Identity;

namespace YourBrand.Meetings.Domain.Entities;

public class User : AggregateRoot<UserId>, IAuditable, IHasTenant
{
    readonly HashSet<OrganizationUser> _organizationUsers = new HashSet<OrganizationUser>();
    readonly HashSet<Organization> _organizations = new HashSet<Organization>();

    public User(UserId id, string name, string email)
        : base(id)
    {
        Id = id;
        Name = name;
        Email = email;
    }
    
    public TenantId TenantId { get; set; }

    public string Name { get; set; }
    public string Email { get; set; }

    public User? CreatedBy { get; set; }
    public UserId? CreatedById { get; set; }
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    public IReadOnlyCollection<Organization> Organizations => _organizations;
    public IReadOnlyCollection<OrganizationUser> OrganizationUsers => _organizationUsers;
}