using System;

namespace YourBrand.IdentityManagement.Contracts;

public record CreateOrganization 
{
    public string Name{ get; init; }
    public string? FriendlyName { get; init; }
}

public record CreateOrganizationResponse(string Id, string Name, string? FriendlyName);

public record OrganizationCreated(string OrganizationId, string Name, string CreatedById);

public record OrganizationUpdated(string OrganizationId, string Name, string UpdatedById);

public record OrganizationDeleted(string OrganizationId, string DeletedById);

public record GetOrganizationResponse(string Id, string Name, string? FriendlyName);