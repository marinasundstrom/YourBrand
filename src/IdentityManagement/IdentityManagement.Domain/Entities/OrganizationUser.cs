using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.IdentityManagement.Domain.Entities;

public class OrganizationUser : IHasTenant
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public TenantId TenantId { get; set; }

    public Tenant Tenant { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public Organization Organization { get; set; }

    public string UserId { get; set; }

    public User User { get; set; }
}