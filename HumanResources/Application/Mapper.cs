using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using YourBrand.HumanResources.Application.Teams;
using YourBrand.HumanResources.Application.Persons;
using YourBrand.HumanResources.Domain.Entities;
using YourBrand.HumanResources.Application.Organizations;

namespace YourBrand.HumanResources.Application;

public static class Mapper
{
    public static OrganizationDto ToDto(this Organization organization) => new OrganizationDto(organization.Id, organization.Name);

    public static PersonDto ToDto(this Person person ) => new PersonDto(person.Id, person.Organization.ToDto(), person.FirstName, person.LastName, person.DisplayName, person.Title, person.Roles.First().Name, person.SSN, person.Email,
                person.Department == null ? null : new DepartmentDto(person.Department.Id, person.Department.Name), person.ReportsTo?.ToDto2(),
                    person.Created, person.LastModified);

    public static Person2Dto ToDto2(this Person person ) => new Person2Dto(person.Id, person.Organization.ToDto(), person.FirstName, person.LastName, person.DisplayName, person.Title,
                person.Department == null ? null : new DepartmentDto(person.Department.Id, person.Department.Name));


    public static TeamDto ToDto(this Team team) => new TeamDto(team.Id, team.Name, team.Description, team.Created, team.LastModified);

    public static TeamMembershipDto ToDto(this TeamMembership teamMembership) => new TeamMembershipDto(teamMembership.Id, teamMembership.Team.ToDto(), teamMembership.Person.ToDto());
}