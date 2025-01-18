using YourBrand.Showroom.Application.Companies;
using YourBrand.Showroom.Application.PersonProfiles.Skills.Queries;

namespace YourBrand.Showroom.Application.PersonProfiles.Employments;

public record EmploymentDto(string Id, CompanyDto Employer, string? Location, string EmploymentType, string Description, DateTime StartDate, DateTime? EndDate);

public record CreateEmploymentDto(string Title, string CompanyId, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description);

public record UpdateEmploymentDto(string Title, string CompanyId, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description);