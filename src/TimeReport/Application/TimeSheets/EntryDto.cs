using YourBrand.TimeReport.Application.Tasks;
using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.TimeSheets;

public record class EntryDto(string Id, ProjectDto Project, TaskDto Task, DateTime Date, double? Hours, string? Description, EntryStatusDto Status);