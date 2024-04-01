using YourBrand.IdentityManagement.Application.Tenants;

namespace YourBrand.IdentityManagement.Application.Organizations;

public record OrganizationDto(string Id, string Name, string? FriendlyName, TenantDto Tenant);