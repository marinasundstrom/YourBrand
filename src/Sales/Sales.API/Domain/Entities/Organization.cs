using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Domain.Entities;

public class Organization : AggregateRoot<string>, IAuditable, IHasTenant
{
    public Organization(string id, string name)
        : base(id)
    {
        Id = id;
        Name = name;
    }

    public TenantId? TenantId { get; set; }

    public string Name { get; set; }

    public User? CreatedBy { get; set; }

    public string? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public string? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    public List<User> Users { get; set; }

    public List<OrganizationUser> OrganizationUsers { get; set; }
}
