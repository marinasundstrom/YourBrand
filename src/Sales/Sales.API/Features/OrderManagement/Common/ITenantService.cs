namespace YourBrand.Sales.Features.Common;

public interface ITenantService
{
    string? TenantId { get; }

    void SetTenantId(string tenantId);
}