namespace YourBrand.HumanResources.Contracts;

public record TeamCreated(string TeamId, string OrganizationId, string Name, string Description);

public record TeamUpdated(string TeamId, string Name, string Description);

public record TeamDeleted(string TeamId);

public record GetTeam(string TeamId);

public record GetTeamResponse(string TeamId, string Name);

public record TeamMemberAdded(string TeamId, string PersonId);

public record TeamMemberRemoved(string TeamId, string PersonId);