using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using YourBrand.IdentityService.Application.Teams;
using YourBrand.IdentityService.Application.Users;
using YourBrand.IdentityService.Domain.Entities;

namespace YourBrand.IdentityService.Application;

public static class Mapper
{
    public static UserDto ToDto(this Person user ) => new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.Roles.First().Name, user.SSN, user.Email,
                user.Department == null ? null : new DepartmentDto(user.Department.Id, user.Department.Name),
                    user.Created, user.LastModified);

    public static TeamDto ToDto(this Team team) => new TeamDto(team.Id, team.Name, team.Description, team.Created, team.LastModified);

    public static TeamMembershipDto ToDto(this TeamMembership teamMembership) => new TeamMembershipDto(teamMembership.Id, teamMembership.Team.ToDto(), teamMembership.Person.ToDto());
}