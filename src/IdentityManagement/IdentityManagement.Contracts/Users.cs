namespace YourBrand.IdentityManagement.Contracts;

public record CreateUser
{
    public string OrganizationId { get; init; }
    public string TenantId { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public string? DisplayName { get; init; }
    public string Role { get; init; }
    public string? Email { get; init; }
};

public record CreateUserResponse(string UserId, string TenantId, string OrganizationId, string FirstName, string LastName, string? DisplayName, string Email);

public record UserCreated(string UserId, string TenantId, string OrganizationId);

public record UserUpdated(string UserId);

public record UserDeleted(string UserId);

public record GetUser(string UserId);

public record GetUserResponse(string UserId, string TenantId, string OrganizationId, string FirstName, string LastName, string? DisplayName, string Email);