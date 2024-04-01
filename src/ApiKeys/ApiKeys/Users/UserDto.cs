namespace YourBrand.ApiKeys.Application.Users;

public record class UserDto(string Id, string FirstName, string LastName, string? DisplayName, string Email, DateTime Created, DateTime? LastModified);