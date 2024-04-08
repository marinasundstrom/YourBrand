namespace YourBrand.Tenancy;

public interface ISettableTenantContext : ITenantContext
{
    void SetTenantId(TenantId tenantId);
}