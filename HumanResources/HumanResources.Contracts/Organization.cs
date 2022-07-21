using System;

namespace YourBrand.HumanResources.Contracts;

public record CreateOrganization(string Name, string? FriendlyName);

public record OrganizationCreated(string OrganizationId, string Name, string CreatedById);

public record OrganizationUpdated(string OrganizationId, string Name, string UpdatedById);

public record OrganizationDeleted(string OrganizationId, string DeletedById);

public record GetOrganizationResponse(string Id, string Name, string? FriendlyName);