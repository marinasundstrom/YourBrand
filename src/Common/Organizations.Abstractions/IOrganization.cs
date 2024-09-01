using YourBrand.Identity;

namespace YourBrand.Domain;

public interface IOrganization
{
    OrganizationId Id { get; }

    bool HasUser(UserId userId);
}