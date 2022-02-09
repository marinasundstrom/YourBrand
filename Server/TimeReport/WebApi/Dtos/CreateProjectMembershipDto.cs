namespace TimeReport.Dtos;

public record class CreateProjectMembershipDto(string UserId, DateTime? From, DateTime? Thru);