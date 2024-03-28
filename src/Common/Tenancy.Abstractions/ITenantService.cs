namespace YourBrand.Tenancy;

public interface ITenantService
{
    string? OrganizationId { get; }
}