using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.Users.Absence;

public record class AbsenceDto(string Id, DateTime Date, string? Description,  ProjectDto? Project);