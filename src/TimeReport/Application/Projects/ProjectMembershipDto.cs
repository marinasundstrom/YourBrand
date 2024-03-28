using YourBrand.TimeReport.Application.Users;

namespace YourBrand.TimeReport.Application.Projects;

public record class ProjectMembershipDto(string Id, ProjectDto Project, UserDto User, DateTime? From, DateTime? Thru);