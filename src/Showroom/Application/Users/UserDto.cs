namespace YourBrand.Showroom.Application.Users;

public record class UserDto(string Id, string FirstName, string LastName, string? DisplayName, string SSN, string Email, DateTimeOffset Created, DateTimeOffset? LastModified);