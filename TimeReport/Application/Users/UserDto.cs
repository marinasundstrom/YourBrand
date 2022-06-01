using YourBrand.TimeReport.Application.Teams;

namespace YourBrand.TimeReport.Application.Users;

public record class UserDto(string Id, string FirstName, string LastName, string? DisplayName, string SSN, string Email, IEnumerable<TeamDto> Teams, DateTime Created, DateTime? Deleted);