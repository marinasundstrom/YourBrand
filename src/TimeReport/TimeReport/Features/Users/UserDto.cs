using YourBrand.TimeReport.Features.Teams;

namespace YourBrand.TimeReport.Features.Users;

public record class UserDto(string Id, string FirstName, string LastName, string? DisplayName, string SSN, string Email, IEnumerable<TeamDto> Teams, DateTimeOffset Created, DateTimeOffset? Deleted);