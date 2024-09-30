using YourBrand.Ticketing.Application.Features.Users;

namespace YourBrand.Ticketing.Application.Features.Projects;

public record class ProjectMembershipDto(string Id, ProjectDto Project, UserDto User, DateTime? From, DateTime? Thru);