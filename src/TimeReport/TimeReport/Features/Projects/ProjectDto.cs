using YourBrand.TimeReport.Features.Organizations;
using YourBrand.TimeReport.Features.Teams;

namespace YourBrand.TimeReport.Features.Projects;

public record class ProjectDto(string Id, string Name, string? Description, OrganizationDto Organization, IEnumerable<TeamDto> Teams);