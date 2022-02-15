namespace Skynet.IdentityService.Application.Users;


public record class UserDto(string Id, string FirstName, string LastName, string? DisplayName, string SSN, string Email, DepartmentDto? Department, DateTime Created, DateTime? Modified);
