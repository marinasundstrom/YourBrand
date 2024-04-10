namespace YourBrand.IdentityManagement.Contracts;

public record CreateTenant
{
    public string Name { get; init; }
    public string? FriendlyName { get; init; }
}

public record GetTenant(string TenantId, string RequestedById);

public record CreateTenantResponse(string TenantId);

public record TenantCreated(string TenantId, string Name);

public record TenantUpdated(string TenantId, string Name);

public record TenantDeleted(string TenantId);

public record GetTenantResponse(string Id, string Name, string? FriendlyName);