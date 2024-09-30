namespace YourBrand.Ticketing.Application.Features.Projects.ProjectGroups;

public record class ProjectGroupDto(string Id, string Name, string? Description, ProjectDto? Project);