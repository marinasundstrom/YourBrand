using System;

namespace Contracts;

public record UserCreated(string UserId);

public record UserUpdated(string UserId);

public record UserDeleted(string UserId);