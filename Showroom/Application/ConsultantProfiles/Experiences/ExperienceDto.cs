using YourBrand.Showroom.Application.ConsultantProfiles.Skills.Queries;

namespace YourBrand.Showroom.Application.ConsultantProfiles.Experiences;

public record ExperienceDto(string Id, string EmployerName, string Title, string CompanyName, string? CompanyLogo, string? Link, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description, IEnumerable<ConsultantProfileSkillDto> Skills);

public record CreateExperienceDto(string Title, string CompanyName, string? CompanyLogo, string? Link, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description);

public record UpdateExperienceDto(string Title, string CompanyName, string? CompanyLogo, string? Link, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description);