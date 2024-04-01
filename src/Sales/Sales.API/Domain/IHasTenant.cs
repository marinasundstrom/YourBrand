using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Domain;

public interface IHasTenant
{
    public TenantId? TenantId { get; set; }
}