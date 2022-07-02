using System;

namespace YourBrand.HumanResources.Contracts;

public record UserCreated(string UserId, string CreatedById);

public record UserUpdated(string UserId, string UpdatedById);

public record UserDeleted(string UserId, string DeletedById);

public record GetUser(string UserId, string RequestedById);

public record GetUserResponse(string UserId, string FirstName, string LastName, string? DisplayName, string SSN, string Email);


public record TeamCreated(string TeamId, string CreatedById);

public record Teampdated(string TeamId, string UpdatedById);

public record TeamDeleted(string TeamId, string DeletedById);

public record GetTeam(string TeamId, string RequestedById);

public record GetTeamResponse(string TeamId, string Name);

public record TeamMemberAdded(string TeamId, string UserId);

public record TeamMemberRemoved(string TeamId, string UserId);