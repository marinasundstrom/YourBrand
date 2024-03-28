using YourBrand.IdentityManagement.Application.Organizations;

namespace YourBrand.IdentityManagement.Application.Users;

public record class UserDto(string Id, OrganizationDto Organization, string FirstName, string LastName, string? DisplayName, string Email, DateTime Created, DateTime? LastModified);

public record class User2Dto(string Id, OrganizationDto Organization, string FirstName, string LastName, string? DisplayName);

