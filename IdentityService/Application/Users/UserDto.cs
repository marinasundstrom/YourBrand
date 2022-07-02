namespace YourBrand.IdentityService.Application.Users;

public record class UserDto(string Id, string FirstName, string LastName, string? DisplayName, string Role, string SSN, string Email, DateTime Created, DateTime? LastModified);
