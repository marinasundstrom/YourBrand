using System;

namespace Skynet.IdentityService.Contracts;

public record UserCreated(string UserId, string CreatedById);

public record UserUpdated(string UserId, string UpdatedById);

public record UserDeleted(string UserId, string DeletedById);

public record GetUser(string UserId, string RequestedById);

public record GetUserResponse(string UserId, string FirstName, string LastName, string? DisplayName, string SSN, string Email);