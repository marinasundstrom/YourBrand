using Microsoft.AspNetCore.Identity;

namespace YourBrand.IdentityManagement.Domain.Entities;

public class OrganizationUser
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string TenantId { get; set; }

    public Tenant Tenant { get; set; }

    public string OrganizationId { get; set; }

    public Organization Organization { get; set; }

    public string UserId { get; set; }

    public User User { get; set; }
}
