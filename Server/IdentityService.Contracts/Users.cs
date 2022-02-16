using System;

namespace Skynet.IdentityService.Contracts;

public record UserCreated(string UserId);

public record UserUpdated(string UserId);

public record UserDeleted(string UserId);