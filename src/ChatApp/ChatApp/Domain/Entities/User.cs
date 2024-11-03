using YourBrand.Auditability;
using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.ChatApp.Domain.Entities;

public class User : AggregateRoot<UserId>, IAuditableEntity<UserId>, IHasTenant
{
    public User(UserId id, string name, string email)
        : base(id)
    {
        Name = name;
        Email = email;
    }

    public string Name { get; set; }

    public TenantId TenantId { get; set; }

    public string Email { get; set; }

    public User? CreatedBy { get; set; }

    public UserId? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    public List<Organization> Organizations { get; set; }

    public List<OrganizationUser> OrganizationUsers { get; set; }
}