using YourBrand.TimeReport.Application.Projects;

namespace YourBrand.TimeReport.Application.Projects.ProjectGroups;

public record class ProjectGroupDto(string Id, string Name, string? Description, ProjectDto? Project);