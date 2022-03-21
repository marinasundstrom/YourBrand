using YourCompany.TimeReport.Application.Users;

namespace YourCompany.TimeReport.Application.Projects;

public record class ProjectMembershipDto(string Id, ProjectDto Project, UserDto User, DateTime? From, DateTime? Thru);