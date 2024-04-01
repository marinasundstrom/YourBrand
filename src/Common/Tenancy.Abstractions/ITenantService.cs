namespace YourBrand.Tenancy;

public interface ITenantService
{
    TenantId? TenantId { get; }

    void SetTenantId(TenantId tenantId);
}
