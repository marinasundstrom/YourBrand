using YourBrand.Showroom.Application.Companies;
using YourBrand.Showroom.Application.PersonProfiles.Skills.Queries;

namespace YourBrand.Showroom.Application.PersonProfiles.Experiences;

public record ExperienceDto(string Id, string? EmployerName, string Title, CompanyDto Company, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description, IEnumerable<PersonProfileSkillDto> Skills);

public record CreateExperienceDto(string Title, string CompanyId, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description);

public record UpdateExperienceDto(string Title, string CompanyId, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description);