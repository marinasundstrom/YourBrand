using YourBrand.Showroom.Application.Companies;
using YourBrand.Showroom.Application.PersonProfiles.Skills.Queries;

namespace YourBrand.Showroom.Application.PersonProfiles.Employments;

public record CreateEmploymentDto(string Title, string CompanyId, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description);

public record UpdateEmploymentDto(string Title, string CompanyId, string? Location, string EmploymentType, DateTime StartDate, DateTime? EndDate, bool Current, bool Highlight, string? Description);