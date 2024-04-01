namespace YourBrand.Tenancy;

public interface IHasTenant
{
    public TenantId TenantId { get; set; }
}