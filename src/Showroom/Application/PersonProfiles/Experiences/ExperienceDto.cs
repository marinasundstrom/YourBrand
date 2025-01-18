using YourBrand.Showroom.Application.Companies;
using YourBrand.Showroom.Application.PersonProfiles.Skills.Queries;

namespace YourBrand.Showroom.Application.PersonProfiles.Experiences;

public record ExperienceDto(string Id, string Type, string? EmployerName, string Title, CompanyDto Company, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description, IEnumerable<EmploymentRoleDto> Roles, IEnumerable<PersonProfileSkillDto> Skills);

public record CreateExperienceDto(string Title, string CompanyId, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description);

public record UpdateExperienceDto(string Title, string CompanyId, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description);

public record EmploymentRoleDto(string Title, string Description, string? Location, DateTime StartDate, DateTime? EndDate, IEnumerable<PersonProfileSkillDto> Skills);