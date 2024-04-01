namespace YourBrand.Sales.Services;

public interface ITenantService
{
    string? TenantId { get; }

    void SetTenantId(string tenantId);
}