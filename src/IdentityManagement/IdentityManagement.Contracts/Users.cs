namespace YourBrand.IdentityManagement.Contracts;

public record IsEmailAlreadyRegistered
{
    public string? Email { get; init; }
}

public record IsEmailAlreadyRegisteredResponse
{
    public bool IsEmailRegistered { get; init; }
}

public record CreateUser
{
    public string OrganizationId { get; init; }
    public required string FirstName { get; init; }
    public string LastName { get; init; }
    public string? DisplayName { get; init; }
    public string Role { get; init; }
    public string? Email { get; init; }
    public string? Password { get; init; }
};

public record CreateUserResponse(string UserId, string TenantId, string FirstName, string LastName, string? DisplayName, string Email);

public record UserCreated(string UserId, string TenantId, string OrganizationId);

public record UserUpdated(string UserId);

public record UserDeleted(string UserId);

public record GetUser(string UserId);

public record GetUserResponse(string UserId, string TenantId, string OrganizationId, string FirstName, string LastName, string? DisplayName, string Email);