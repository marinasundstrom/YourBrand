using YourBrand.ChatApp.Domain.ValueObjects;
using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.ChatApp.Domain.Entities;

public class Organization : AggregateRoot<OrganizationId>, IAuditable, IHasTenant
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
}