using YourBrand.TimeReport.Features.Users;

namespace YourBrand.TimeReport.Features.Projects;

public record class ProjectMembershipDto(string Id, ProjectDto Project, UserDto User, DateTime? From, DateTime? Thru);