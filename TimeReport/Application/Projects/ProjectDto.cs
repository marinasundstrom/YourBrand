using YourBrand.TimeReport.Application.Organizations;
using YourBrand.TimeReport.Application.Teams;

namespace YourBrand.TimeReport.Application.Projects;

public record class ProjectDto(string Id, string Name, string? Description, OrganizationDto Organization, IEnumerable<TeamDto> Teams);