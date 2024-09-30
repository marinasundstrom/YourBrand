namespace YourBrand.Ticketing.Application.Features.Projects;

public record class CreateProjectMembershipDto(string UserId, DateTime? From, DateTime? Thru);