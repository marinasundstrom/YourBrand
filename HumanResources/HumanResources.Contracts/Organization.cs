using System;

namespace YourBrand.HumanResources.Contracts;

public record OrganizationCreated(string OrganizationId, string Name, string CreatedById);

public record OrganizationUpdated(string OrganizationId, string Name, string UpdatedById);

public record OrganizationDeleted(string OrganizationId, string DeletedById);