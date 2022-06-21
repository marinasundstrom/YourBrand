using YourBrand.IdentityService.Application.Users;

namespace YourBrand.IdentityService.Application.Teams;

public record class TeamMembershipDto(string Id, TeamDto Team, UserDto User);