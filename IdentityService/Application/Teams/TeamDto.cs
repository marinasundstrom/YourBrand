namespace YourBrand.IdentityService.Application.Teams;

public record class TeamDto(string Id, string Name, string? Description, DateTime Created, DateTime? LastModified);
