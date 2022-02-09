using Skynet.TimeReport.Application.Users;

namespace Skynet.TimeReport.Application.Projects;

public record class ProjectMembershipDto(string Id, ProjectDto Project, UserDto User, DateTime? From, DateTime? Thru);