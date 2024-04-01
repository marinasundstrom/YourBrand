using YourBrand.IdentityManagement.Application.Organizations;
using YourBrand.IdentityManagement.Application.Tenants;
using YourBrand.IdentityManagement.Application.Users;
using YourBrand.IdentityManagement.Domain.Entities;

namespace YourBrand.IdentityManagement.Application;

public static class Mapper
{
    public static TenantDto ToDto(this Tenant tenant) => new(tenant.Id, tenant.Name, tenant.FriendlyName);

    public static OrganizationDto ToDto(this Organization organization) => new(organization.Id, organization.Name, organization.FriendlyName, organization.Tenant?.ToDto());

    public static UserDto ToDto(this User user) => new(user.Id, user.Tenant?.ToDto(), user.FirstName, user.LastName, user.DisplayName, user.Email,
                    user.Created, user.LastModified);
}