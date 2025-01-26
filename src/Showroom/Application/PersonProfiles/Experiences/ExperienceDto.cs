using System.Text.Json.Serialization;

using YourBrand.Showroom.Application.Companies;
using YourBrand.Showroom.Application.PersonProfiles.Skills.Queries;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Application.PersonProfiles.Experiences;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(EmploymentDto), "Employment")]
[JsonDerivedType(typeof(AssignmentDto), "Assignment")]
[JsonDerivedType(typeof(ProjectDto), "Project")]
[JsonDerivedType(typeof(CareerBreakDto), "CareerBreak")]
public abstract record ExperienceDto(
    ExperienceType Type,
    DateTime StartDate, DateTime? EndDate,
    string? Description);

public record CareerBreakDto(
    string Id,
    DateTime StartDate, DateTime? EndDate,
    string? Description)
    : ExperienceDto(ExperienceType.CareerBreak, StartDate, EndDate, Description);

public record EmploymentDto(
    string Id,
    CompanyDto Employer,
    string? Location,
    EmploymentType EmploymentType,
    DateTime StartDate, DateTime? EndDate,
    string? Description,
    IEnumerable<RoleDto> Roles, 
    IEnumerable<PersonProfileSkillDto> Skills)
    : ExperienceDto(ExperienceType.Employment, StartDate, EndDate, Description);

public record AssignmentDto(
    string Id,
    EmploymentDto Employment,
    CompanyDto? Company,
    string? Location,
    AssignmentType AssignmentType,
    DateTime StartDate, DateTime? EndDate,
    string? Description,
    IEnumerable<RoleDto> Roles,
    IEnumerable<PersonProfileSkillDto> Skills)
    : ExperienceDto(ExperienceType.Assignment, StartDate, EndDate, Description);

public record ProjectDto(
    string Id,
    string Name,
    EmploymentDto? Employment,
    AssignmentDto? Assignment,
    CompanyDto? Company,
    string? Location,
    DateTime StartDate, DateTime? EndDate,
    string? Description)
    : ExperienceDto(ExperienceType.Project, StartDate, EndDate, Description);

public record RoleDto(
    string Id,
    string Title,
    //EmploymentDto? Employment,
    //AssignmentDto? Assignment,
    string? Location,
    DateTime StartDate, DateTime? EndDate,
    string? Description,
    IEnumerable<PersonProfileSkillDto> Skills);

public record CreateExperienceDto(ExperienceType Type, string Title, string CompanyId, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description);

public record UpdateExperienceDto(ExperienceType Type, string Title, string CompanyId, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description);