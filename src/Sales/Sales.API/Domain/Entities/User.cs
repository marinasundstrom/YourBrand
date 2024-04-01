using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Domain.Entities;

public class User : AggregateRoot<string>, IAuditable, IHasTenant
{
    public User(string id, string name, string email)
        : base(id)
    {
        Id = id;
        Name = name;
        Email = email;
    }

    public string Name { get; set; }

    public TenantId? TenantId { get; set; }

    public string Email { get; set; }

    public User? CreatedBy { get; set; }

    public string? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public string? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }

    public List<Organization> Organizations { get; set; }

    public List<OrganizationUser> OrganizationUsers { get; set; }
}