using System;

namespace YourBrand.IdentityManagement.Contracts;

public record CreateTenant 
{
    public string Name{ get; init; }
    public string? FriendlyName { get; init; }
}

public record GetTenant(string TenantId, string RequestedById);

public record CreateTenantResponse(string Id, string Name, string? FriendlyName);

public record TenantCreated(string TenantId, string Name, string CreatedById);

public record TenantUpdated(string TenantId, string Name, string UpdatedById);

public record TenantDeleted(string TenantId, string DeletedById);

public record GetTenantResponse(string Id, string Name, string? FriendlyName);