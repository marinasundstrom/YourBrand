using YourBrand.HumanResources.Application.Users;

namespace YourBrand.HumanResources.Application.Teams;

public record class TeamMembershipDto(string Id, TeamDto Team, UserDto User);