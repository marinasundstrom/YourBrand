namespace YourBrand.HumanResources.Application.Teams;

public record class TeamDto(string Id, string Name, string? Description, DateTimeOffset Created, DateTimeOffset? LastModified);