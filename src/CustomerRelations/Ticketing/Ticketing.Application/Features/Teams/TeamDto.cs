using YourBrand.Ticketing.Application.Features.Users;

namespace YourBrand.Ticketing.Application.Features.Teams
;

public record TeamDto
(
    string Id,
    string Name,
    IEnumerable<TeamMemberDto> Members
);

public record TeamMemberDto(string Id, string FirstName, string LastName);

public record TeamMembershipDto(string Id, UserDto User);