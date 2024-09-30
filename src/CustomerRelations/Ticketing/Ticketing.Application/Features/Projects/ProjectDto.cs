using YourBrand.Ticketing.Application.Features.Teams;
using YourBrand.Ticketing.Application.Features.Organizations;

namespace YourBrand.Ticketing.Application.Features.Projects;

public record class ProjectDto(int Id, string Name, string? Description, OrganizationDto Organization, IEnumerable<TeamDto> Teams);