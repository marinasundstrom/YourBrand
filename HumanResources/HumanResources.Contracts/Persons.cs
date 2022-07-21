using System;

namespace YourBrand.HumanResources.Contracts;

public record CreatePerson(string OrganizationId, string FirstName, string LastName, string? DisplayName, string Title, string Role, string SSN, string Email, string DepartmentId, string? ReportsTo, string Password);

public record PersonCreated(string PersonId, string CreatedById);

public record PersonUpdated(string PersonId, string UpdatedById);

public record PersonDeleted(string PersonId, string DeletedById);

public record GetPerson(string PersonId, string RequestedById);

public record GetPersonResponse(string PersonId, string OrganizationId, string FirstName, string LastName, string? DisplayName, string SSN, string Email);
