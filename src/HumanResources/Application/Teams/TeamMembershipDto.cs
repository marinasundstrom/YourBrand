using YourBrand.HumanResources.Application.Persons;

namespace YourBrand.HumanResources.Application.Teams;

public record class TeamMembershipDto(string Id, TeamDto Team, PersonDto Person);