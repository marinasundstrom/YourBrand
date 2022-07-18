using System;

namespace YourBrand.HumanResources.Contracts;

public record PersonCreated(string PersonId, string CreatedById);

public record PersonUpdated(string PersonId, string UpdatedById);

public record PersonDeleted(string PersonId, string DeletedById);

public record GetPerson(string PersonId, string RequestedById);

public record GetPersonResponse(string PersonId, string FirstName, string LastName, string? DisplayName, string SSN, string Email);
