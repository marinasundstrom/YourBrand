using System;

namespace YourBrand.HumanResources.Contracts;

public record PersonCreated(string PersonId, string CreatedById);

public record PersonUpdated(string PersonId, string UpdatedById);

public record PersonDeleted(string PersonId, string DeletedById);

public record GetPerson(string PersonId, string RequestedById);

public record GetPersonResponse(string PersonId, string FirstName, string LastName, string? DisplayName, string SSN, string Email);


public record TeamCreated(string TeamId, string CreatedById);

public record Teampdated(string TeamId, string UpdatedById);

public record TeamDeleted(string TeamId, string DeletedById);

public record GetTeam(string TeamId, string RequestedById);

public record GetTeamResponse(string TeamId, string Name);

public record TeamMemberAdded(string TeamId, string PersonId);

public record TeamMemberRemoved(string TeamId, string PersonId);