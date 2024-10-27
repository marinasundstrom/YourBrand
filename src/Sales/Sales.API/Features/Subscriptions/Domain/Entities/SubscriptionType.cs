using YourBrand.Domain;
using YourBrand.Sales.Domain.ValueObjects;

using YourBrand.Tenancy;

namespace YourBrand.Sales.Domain.Entities;

public sealed class SubscriptionType : Entity<int>, IAuditable, IHasTenant
{
    protected SubscriptionType()
    {
    }

    public SubscriptionType(string name, string handle, string? description)
    {
        Name = name;
        Handle = handle;
        Description = description;
    }

    public SubscriptionType(int id, string name, string handle, string? description)
    {
        Id = id;
        Name = name;
        Handle = handle;
        Description = description;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public string Name { get; set; } = null!;

    public string Handle { get; set; } = null!;

    public string? Description { get; set; }

    public int? Type { get; set; } = null!;

    public User? CreatedBy { get; set; }

    public UserId? CreatedById { get; set; }

    public DateTimeOffset Created { get; set; }

    public User? LastModifiedBy { get; set; }

    public UserId? LastModifiedById { get; set; }

    public DateTimeOffset? LastModified { get; set; }
}