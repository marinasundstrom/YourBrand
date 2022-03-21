namespace YourCompany.Showroom.Application.ConsultantProfiles.Experiences;

public record ExperienceDto(string Id, string Title, string CompanyName, string? Location, DateTime StartDate, DateTime? EndDate, string? Description);

public record CreateExperienceDto(string Title, string CompanyName, string? Location, DateTime StartDate, DateTime? EndDate, string? Description);

public record UpdateExperienceDto(string Title, string CompanyName, string? Location, DateTime StartDate, DateTime? EndDate, string? Description);