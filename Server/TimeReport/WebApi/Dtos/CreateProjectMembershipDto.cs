namespace Skynet.TimeReport.Dtos;

public record class CreateProjectMembershipDto(string UserId, DateTime? From, DateTime? Thru);