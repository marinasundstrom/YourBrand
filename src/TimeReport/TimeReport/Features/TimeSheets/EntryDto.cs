using YourBrand.TimeReport.Features.Tasks;
using YourBrand.TimeReport.Features.Projects;

namespace YourBrand.TimeReport.Features.TimeSheets;

public record class EntryDto(string Id, ProjectDto Project, TaskDto Task, DateTime Date, double? Hours, string? Description, EntryStatusDto Status);