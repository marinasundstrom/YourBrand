namespace Skynet.Showroom.Application.ConsultantProfiles;

public record ExperienceDto(string Id, string Title, string CompanyName, string? Location, DateTime StartDate, DateTime? EndDate, string? Description);