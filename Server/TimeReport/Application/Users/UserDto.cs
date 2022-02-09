namespace TimeReport.Application.Users;

public record class UserDto(string Id, string FirstName, string LastName, string? DisplayName, string SSN, string Email, DateTime Created, DateTime? Deleted);