using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using YourBrand.HumanResources.Application.Teams;
using YourBrand.HumanResources.Application.Persons;
using YourBrand.HumanResources.Domain.Entities;

namespace YourBrand.HumanResources.Application;

public static class Mapper
{
    public static PersonDto ToDto(this Person person ) => new PersonDto(person.Id, person.FirstName, person.LastName, person.DisplayName, person.Roles.First().Name, person.SSN, person.Email,
                person.Department == null ? null : new DepartmentDto(person.Department.Id, person.Department.Name),
                    person.Created, person.LastModified);

    public static TeamDto ToDto(this Team team) => new TeamDto(team.Id, team.Name, team.Description, team.Created, team.LastModified);

    public static TeamMembershipDto ToDto(this TeamMembership teamMembership) => new TeamMembershipDto(teamMembership.Id, teamMembership.Team.ToDto(), teamMembership.Person.ToDto());
}