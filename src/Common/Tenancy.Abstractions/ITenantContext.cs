namespace YourBrand.Tenancy;

public interface ITenantContext
{
    TenantId? TenantId { get; }
}