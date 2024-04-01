namespace YourBrand.HumanResources.Contracts;

public record TeamCreated(string TeamId, string OrganizationId, string Name, string Description, string CreatedById);

public record TeamUpdated(string TeamId, string Name, string Description, string UpdatedById);

public record TeamDeleted(string TeamId, string DeletedById);

public record GetTeam(string TeamId, string RequestedById);

public record GetTeamResponse(string TeamId, string Name);

public record TeamMemberAdded(string TeamId, string PersonId);

public record TeamMemberRemoved(string TeamId, string PersonId);