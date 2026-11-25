using YourBrand.TimeReport.Features.Projects;

namespace YourBrand.TimeReport.Features.Users.Absence;

public record class AbsenceDto(string Id, DateTime Date, string? Description, ProjectDto? Project);